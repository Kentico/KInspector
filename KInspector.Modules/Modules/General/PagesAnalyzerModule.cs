using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Text;
using System.Text.RegularExpressions;
using Kentico.KInspector.Core;

namespace Kentico.KInspector.Modules
{
    public class PagesAnalyzerModule : IModule
    {
        #region "Constants"

        /// <summary>
        /// Default favicon path to be tried when no explicit favicon is provided.
        /// </summary>
        private const string DEFAULT_FAVICON = "/favicon.ico";


        /// <summary>
        /// Regular expression for matching <c>link</c> elements with certain <c>rel</c> attribute value.
        /// </summary>
        private const string FAVICON_LINK_ELEMENT_REGEX_PATTERN = "(?<linkElement><link[^>]+rel=\"\\s*({0})\\s*\"[^>]*>)";

        #endregion


        #region "IModule methods"

        public ModuleMetadata GetModuleMetadata()
        {
            return new ModuleMetadata
            {
                Name = "Detailed analysis of all rendered pages (takes a while)",
                SupportedVersions = new[] { 
                    new Version("6.0"),
                    new Version("7.0"),
                    new Version("8.0"), 
                    new Version("8.1"), 
                    new Version("8.2")
                },
                Comment = @"Checks response time, response status, HTML size, ViewState size, number of links, favicons and Apple touch icons on the page. 

Needs to have preset Url in the setup screen.

The favicon is checked as follows: The markup is analyzed for presence of <link> element specifying either 'icon' or 'shortcut icon' in its rel attribute.
If multiple elements specifying favicon are found, all their hrefs are checked.
When no explicit favicon is provided, the 'favicon.ico' in domain root is checked.

The Apple touch icons are checked as follows: The markup is analyzed for presence of <link> elements specifying either 'apple-touch-icon' or 'apple-touch-icon-precomposed' in their rel attribute.
If multiple elements specifying touch icons are found, all their hrefs are checked.
When no explicit touch icon is provided, no implicit icon is assumed, since there are many icon names to be tried (depending on device resolution, precomposed/standard icon version).
For details on how the devices try to retrieve the implicit touch icon see
https://mathiasbynens.be/notes/touch-icons#no-html

It is a good practice to provide explicit touch icons in the markup since only some browsers in some versions try to load implicit icons and the implicit paths differ in preferred resolution (the fallback with no version in its name is supported though).
Note: Although it may seem that touch icon is for Apple devices only, this is not true. Android, BlackBerry and some other devices support it as well.",
                };
        }


        public ModuleResults GetResults(IInstanceInfo instanceInfo)
        {
            var results = new ModuleResults();

            var dbService = instanceInfo.DBService;
            var sql = $@"SELECT s.SiteID FROM CMS_Site AS s LEFT JOIN CMS_SiteDomainAlias AS sa ON s.SiteID = sa.SiteID
                            WHERE ('{instanceInfo.Uri}' LIKE '%' + s.SiteDomainName + '%'
                            OR '{instanceInfo.Uri}' LIKE '%' + sa.SiteDomainAliasName + '%') AND s.SiteStatus = N'RUNNING'";

            var siteIDRaw = dbService.ExecuteAndGetScalar<string>(sql);
            int siteID = 0;
            if(!int.TryParse(siteIDRaw, out siteID))
            {
                results.Result = $"No site found matching the URL: {instanceInfo.Uri}";
                results.Status = Status.Error;
                return results;
            }
            
            
            var aliases = dbService.ExecuteAndGetTableFromFile("PagesAnalyzerModule.sql",
                new SqlParameter("SiteId", siteID.ToString()));
            var allLinks = new Dictionary<string, List<string>>();

            Dictionary<string, string> faviconAvailabilityCache = new Dictionary<string, string>();
            Dictionary<string, string> touchIconAvailabilityCache = new Dictionary<string, string>();

            // Workaround to ignore invalid SSL certificates
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback((a, b, c, d) => { return true; });

            foreach (DataRow alias in aliases.Rows)
            {
                var redirected = alias["Redirected"].ToString().ToLower();
                switch (redirected)
                {
                    // If version 8 and higher is used and page is redirected to first child
                    case "1":
                    case "true":
                        alias["Redirected"] = "True";
                        continue;
                    case "doesnotexist": // If version 7 and lower is used, database column does not exist
                    default:
                        alias["Redirected"] = "False";
                        break;
                }

                var aliasPath = alias["AliasPath"].ToString().TrimStart('/');

                // In case of MVC page skip
                if (aliasPath.StartsWith("ROUTE"))
                {
                    continue;
                }

                var uri = new Uri(instanceInfo.Uri, aliasPath + ".aspx");
                var html = string.Empty;
                try
                {
                    HttpWebRequest request = WebRequest.CreateHttp(uri);
                    Stopwatch sw = Stopwatch.StartNew();
                    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                    {
                        // Get size of the request
                        using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                        {
                            html = sr.ReadToEnd();
                            alias["Response"] = response.StatusCode.ToString();
                        }
                        alias["Response type"] = response.ContentType;
                    }
                    sw.Stop();
                    alias["Response Time [ms]"] = sw.ElapsedMilliseconds;
                }
                catch (WebException e)
                {
                    // Probably 404
                    alias["Response"] = e.Status.ToString();
                }

                alias["HTML Size [KB]"] = html.Length / 1024;

                var viewState = Regex.Match(html, "(?<=__VIEWSTATE\" value=\")(?<val>.*?)(?=\")").Groups["val"].Value;
                alias["ViewState Size [KB]"] = viewState.Length / 1024;

                var linksRegex = new Regex("(href|src)=\"(/[^\"]+)|url\\('(/[^\']+)|url\\(\"(/[^\"]+)");
                var links = linksRegex.Matches(html);

                // Evaluate favicon availability
                alias["Favicon"] = EvaluateFaviconAvailability(html, uri, faviconAvailabilityCache);

                // Evaluate Apple touch icon  and precomposed icon availability
                alias["Apple Touch Icon"] = EvaluateAppleTouchIconAvailability(html, uri, touchIconAvailabilityCache);
                alias["Apple Touch Icon Precomposed"] = EvaluateAppleTouchIconAvailability(html, uri, touchIconAvailabilityCache, true);

                alias["Images without alt"] = GetImagesWithoutAlt(html);
                
                // Evaluate links count
                alias["Link count"] = links.Count;
                if (links.Count > 0)
                {
                    foreach (Match match in links)
                    {
                        string link = match.Groups[2].ToString();
                        if (string.IsNullOrEmpty(link))
                        {
                            link = match.Groups[3].ToString();
                            if (string.IsNullOrEmpty(link))
                            {
                                link = match.Groups[4].ToString();
                            }
                        }

                        if (!link.ToLower().Contains("/webresource") && !link.ToLower().Contains("/scriptresource"))
                        {
                            if (allLinks.ContainsKey(link))
                            {
                                allLinks[link].Add(aliasPath);
                            }
                            else
                            {
                                allLinks[link] = new List<string> { aliasPath };
                            }
                        }
                    }
                }
            }

            foreach (var linkList in allLinks)
            {
                // all the links are here, TODO: request them and get the response status
            }

            results.Result = aliases;

            return results;
        }

        #endregion


        #region "Private methods"

        /// <summary>
        /// Evaluates availability of all favicons within given <paramref name="html"/>.
        /// </summary>
        /// <param name="html">HTML to be evaluated.</param>
        /// <param name="baseUri">Base URI to be used when favicon's href is relative path.</param>
        /// <param name="faviconAvailabilityCache">Cache containing pairs of (absolute URI, availability status) of already probed favicon URIs.</param>
        /// <returns>Status of all favions contained in the <paramref name="html"/>.</returns>
        private string EvaluateFaviconAvailability(string html, Uri baseUri, Dictionary<string, string> faviconAvailabilityCache)
        {
            var faviconHrefs = GetFaviconHrefs(html);
            bool defaultFavicon = false;
            if (faviconHrefs.Count == 0)
            {
                // If no favicon is specified in the markup, use the default
                faviconHrefs.Add(DEFAULT_FAVICON);
                defaultFavicon = true;
            }

            StringBuilder res = new StringBuilder();
            foreach (string faviconHref in faviconHrefs)
            {
                Uri faviconUri = GetUri(baseUri, faviconHref);
                if (faviconAvailabilityCache.ContainsKey(faviconUri.AbsoluteUri))
                {
                    res.Append(faviconAvailabilityCache[faviconUri.AbsoluteUri]);
                }
                else
                {
                    bool faviconAvailable = ProbeUri(faviconUri);
                    if (faviconAvailable)
                    {
                        faviconAvailabilityCache[faviconUri.AbsoluteUri] = (defaultFavicon) ? "OK (Default)\n" : $"OK ('{faviconHref}')\n";
                    }
                    else
                    {
                        faviconAvailabilityCache[faviconUri.AbsoluteUri] = (defaultFavicon) ? "NOT SPECIFIED\n" : $"MISSING ('{faviconHref}')\n";
                    }
                    res.Append(faviconAvailabilityCache[faviconUri.AbsoluteUri]);
                }
            }

            return res.ToString();
        }


        /// <summary>
        /// Evaluates availability of all Apple touch icons within given <paramref name="html"/>.
        /// </summary>
        /// <param name="html">HTML to be evaluated.</param>
        /// <param name="baseUri">Base URI to be used when touch icon's href is relative path.</param>
        /// <param name="touchIconAvailabilityCache">Cache containing pairs of (absolute URI, availability status) of already probed icon URIs.</param>
        /// <param name="evaluatePrecomposed">Whether to evaluate standard or precomposed Apple touch icons.</param>
        /// <returns>Status of all favicons contained in the <paramref name="html"/>.</returns>
        private string EvaluateAppleTouchIconAvailability(string html, Uri baseUri, Dictionary<string, string> touchIconAvailabilityCache, bool evaluatePrecomposed = false)
        {
            var appleTouchIconHrefs = evaluatePrecomposed ? GetAppleTouchIconPrecomposedHrefs(html) : GetAppleTouchIconHrefs(html);

            StringBuilder res = new StringBuilder();
            if (appleTouchIconHrefs.Count == 0)
            {
                res.Append("NOT SPECIFIED\n");
            }
            else
            {
                foreach (string iconHref in appleTouchIconHrefs)
                {
                    Uri iconUri = GetUri(baseUri, iconHref);
                    if (touchIconAvailabilityCache.ContainsKey(iconUri.AbsoluteUri))
                    {
                        res.Append(touchIconAvailabilityCache[iconUri.AbsoluteUri]);
                    }
                    else
                    {
                        bool faviconAvailable = ProbeUri(iconUri);
                        if (faviconAvailable)
                        {
                            touchIconAvailabilityCache[iconUri.AbsoluteUri] = $"OK ('{iconHref}')\n";
                        }
                        else
                        {
                            touchIconAvailabilityCache[iconUri.AbsoluteUri] = $"MISSING ('{iconHref}')\n";
                        }
                        res.Append(touchIconAvailabilityCache[iconUri.AbsoluteUri]);
                    }
                }
            }

            return res.ToString();
        }


        /// <summary>
        /// Tests whether given <paramref name="uri"/> exists and reads the content.
        /// </summary>
        /// <param name="uri">URI to be probed.</param>
        /// <returns>True if the probing is successful (URI exists and content retrieval succeeds), false otherwise.</returns>
        private bool ProbeUri(Uri uri)
        {
            try
            {
                HttpWebRequest request = WebRequest.CreateHttp(uri);
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    // Try to retrieve the content 
                    using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                    {
                        var dummyRead = sr.ReadToEnd();
                    }
                }
                return true;
            }
            catch (WebException)
            {
                return false;
            }
        }


        /// <summary>
        /// Gets URI from the provided <paramref name="baseUri"/> and <paramref name="href"/>.
        /// </summary>
        /// <param name="baseUri">Base URI, used when <paramref name="href"/> value is relative.</param>
        /// <param name="href">Href attribute value of an element (abolute or relative URI).</param>
        /// <returns>URI created from the base URI and href.</returns>
        /// <exception cref="InvalidOperationException">Thrown when baseUri and href do not produce a valid URI.</exception>
        private Uri GetUri(Uri baseUri, string href)
        {
            Uri result;
            if (Uri.TryCreate(baseUri, href, out result))
            {
                return result;
            }

            throw new InvalidOperationException($"[PagesAnalyzerModule.GetUri]: The base URI '{baseUri}' and href '{href}' do not produce a valid URI.");
        }


        /// <summary>
        /// Gets values of <c>href</c> attribute of all Apple touch icon precomosed <c>link</c> elements within <paramref name="html"/>.
        /// </summary>
        /// <param name="html">HTML containing any number of <c>link</c> elements.</param>
        /// <returns>Set containing all matched <c>href</c> element values.</returns>
        private ISet<string> GetAppleTouchIconHrefs(string html)
        {
            return GetLinkElementHrefs(html, "\\s*apple-touch-icon\\s*");
        }


        /// <summary>
        /// Gets values of <c>href</c> attribute of all Apple touch icon <c>link</c> elements within <paramref name="html"/>.
        /// </summary>
        /// <param name="html">HTML containing any number of <c>link</c> elements.</param>
        /// <returns>Set containing all matched <c>href</c> element values.</returns>
        private ISet<string> GetAppleTouchIconPrecomposedHrefs(string html)
        {
            return GetLinkElementHrefs(html, "\\s*apple-touch-icon-precomposed\\s*");
        }


        /// <summary>
        /// Gets values of <c>href</c> attribute of all favicon <c>link</c> elements within <paramref name="html"/>.
        /// </summary>
        /// <param name="html">HTML containing any number of <c>link</c> elements.</param>
        /// <returns>Set containing all matched <c>href</c> element values.</returns>
        private ISet<string> GetFaviconHrefs(string html)
        {
            return GetLinkElementHrefs(html, "\\s*(icon|shortcut\\s+icon)\\s*");
        }


        /// <summary>
        /// Gets values of <c>href</c> attribute of all <c>link</c> elements within <paramref name="html"/>,
        /// which have <c>rel</c> attribute value equal to <paramref name="linkRelValueRegex"/>.
        /// </summary>
        /// <param name="html">HTML containing any number of <c>link</c> elements.</param>
        /// <param name="linkRelValueRegex">Value of the <c>rel</c> attribute the <c>link</c> has to match, which is substituted to regular expression
        /// (can use regular expression syntax).</param>
        /// <returns>Set containing all matched <c>href</c> element values.</returns>
        private ISet<string> GetLinkElementHrefs(string html, string linkRelValueRegex)
        {
            HashSet<string> result = new HashSet<string>();
            var linkElementRegex = new Regex(string.Format(FAVICON_LINK_ELEMENT_REGEX_PATTERN, linkRelValueRegex));
            foreach (Match match in linkElementRegex.Matches(html))
            {
                string linkElement = match.Groups["linkElement"].Value;
                result.Add(GetHrefAttributeValue(linkElement));
            }

            return result;
        }


        /// <summary>
        /// Locates the <c>href</c> attribute within <paramref name="elementXml"/>
        /// and returns its value.
        /// </summary>
        /// <param name="elementXml">Element XML (eg. &lt;link href="..."&gt;)</param>
        /// <returns>Value of <c>href</c> attribute, or null if not present.</returns>
        private string GetHrefAttributeValue(string elementXml)
        {
            var hrefValueRegex = new Regex("href=\"(?<hrefValue>[^\"]+)\"");
            var hrefValueGroup = hrefValueRegex.Match(elementXml).Groups["hrefValue"];
            if (hrefValueGroup.Success)
            {
                return hrefValueGroup.Value;
            }

            return null;
        }


        private string GetImagesWithoutAlt(string html)
        {
            var regexPattern = @"<img(?![^>]*\balt=)[^>]*?>";
            var regex = new Regex(regexPattern, RegexOptions.IgnoreCase);

            var matches = regex.Matches(html).Count;
            if (matches > 0)
            {
                return matches.ToString();
            }

            return "NO";
        }

        #endregion
    }
}

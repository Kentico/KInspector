using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Runtime.Remoting.Messaging;
using System.Web;
using System.Web.Configuration;
using Kentico.KInspector.Core;

namespace Kentico.KInspector.Modules
{
    public class SecurityAppSettingsModule : IModule
    {
        private const string RECOMMENDED_VALUE_TRUE = "True";
        private const string RECOMMENDED_VALUE_FALSE = "False";
        private const string VALUE_NOT_SET = "No value set";

        public ModuleMetadata GetModuleMetadata()
        {
            return new ModuleMetadata
            {
                Name = "Security settings in web.config",
                Comment = 
@"Checks the following security settings in web.config:
- Compilation debug
- Tracing
- Custom errors
- Cookieless authentication
- Session fixation
- CSRF protection check 
- Http only cookies
- Viewstate (MAC) validation
- Hash string salt
- SA in CMSConnectionString
- Click jacking protection override",
                SupportedVersions = new[] {
                    new Version("7.0"),
                    new Version("8.0"),
                    new Version("8.1"),
                    new Version("8.2"),
                    new Version("9.0"),
                    new Version("10.0"),
                    new Version("11.0"),
                    new Version("12.0")
                },
                Category = "Security",
            };
        }

        public ModuleResults GetResults(IInstanceInfo instanceInfo)
        {
            // Prepare result
            DataTable result = new DataTable();
            result.Columns.Add("Element / Key", typeof(string));
            result.Columns.Add("Value", typeof(string));
            result.Columns.Add("Recommended value", typeof(string));

            // Update web.config path with "CMS" folder for Kentico 8 and newer versions
            var kenticoVersion = instanceInfo.Version;
            string pathToWebConfig = instanceInfo.Directory.ToString();

            if ((kenticoVersion >= new Version("8.0")) && !(instanceInfo.Directory.ToString().EndsWith("\\CMS\\") || instanceInfo.Directory.ToString().EndsWith("\\CMS")))
            {
                pathToWebConfig += "\\CMS";
            }

            var fileMap = new ExeConfigurationFileMap { ExeConfigFilename = pathToWebConfig + "\\web.config" };
            var configuration = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);

            # region "Debug mode"
            var compilationNode = (CompilationSection)configuration.GetSection("system.web/compilation");
            bool debugMode = compilationNode.Debug;

            if (debugMode)
            {
                result.Rows.Add("Debug (<compilation debug=\"...)", debugMode.ToString(), RECOMMENDED_VALUE_FALSE);
            }

            # endregion

            # region "Tracing"

            var traceNode = (TraceSection)configuration.GetSection("system.web/trace");
            bool tracing = traceNode.Enabled;

            if (tracing)
            {
                result.Rows.Add("Tracing (<trace enabled=\"...)", tracing.ToString(), RECOMMENDED_VALUE_FALSE);
            }

            # endregion

            # region "Custom errors"

            var customErrorsNode = (CustomErrorsSection)configuration.GetSection("system.web/customErrors");
            var customErrors = customErrorsNode.Mode;

            if (customErrors != CustomErrorsMode.On)
            {
                result.Rows.Add("Custom errors (<customErrors mode=\"...)", customErrors.ToString(), CustomErrorsMode.On.ToString());
            }

            # endregion

            # region "Cookieless authentication"

            var authNode = (AuthenticationSection)configuration.GetSection("system.web/authentication");
            var cookieless = authNode.Forms.Cookieless;

            if (cookieless != HttpCookieMode.UseCookies) // Auto? Device?
            {
                result.Rows.Add("Cookieless auth (<forms cookieless=\"...)", cookieless.ToString(), HttpCookieMode.UseCookies.ToString());
            }

            # endregion

            #region "HttpOnlyCookies"

            var httpOnlyCookiesNode = (HttpCookiesSection)configuration.GetSection("system.web/httpCookies");
            bool httpOnlyCookies = httpOnlyCookiesNode.HttpOnlyCookies;

            if (!httpOnlyCookies)
            {
                result.Rows.Add("HttpOnlyCookies (<httpCookies httpOnlyCookies=\"...)", httpOnlyCookies.ToString(), RECOMMENDED_VALUE_TRUE);
            }

            # endregion

            # region "Viewstate (MAC) validation"

            var pagesNode = (PagesSection)configuration.GetSection("system.web/pages");
            bool viewstate = pagesNode.EnableViewState;
            bool viewstatemac = pagesNode.EnableViewStateMac;

            if (!viewstate)
            {
                result.Rows.Add("Viewstate (<pages EnableViewState=\"...)", viewstate.ToString(), RECOMMENDED_VALUE_TRUE);
            }

            if (!viewstatemac)
            {
                result.Rows.Add("Viewstate MAC (<pages EnableViewStateMac=\"...)", viewstatemac.ToString(), RECOMMENDED_VALUE_TRUE);
            }

            #endregion

            #region "Using SA account for SQL connection"

            var connectionString = configuration.ConnectionStrings.ConnectionStrings["CMSConnectionString"];

            if (connectionString != null)
            {
                var usingServerAdminAccount = connectionString.ConnectionString.ToLower().Contains("user id=sa;");
                if (usingServerAdminAccount)
                {
                    result.Rows.Add("CMS Connection string is using SA account", "User ID=SA;", "Use integrated security or a specific user");
                }
            }
            else
            {
                result.Rows.Add("CMS Connection string is not present", VALUE_NOT_SET, "Add a CMSConnectionString");
            }


            #endregion

            #region Recommended key values

            // Create list of (tuple) keys to check in format:
            // Item1 Key name
            // Item2 Recommended value test (predicate)
            // Item3 "display name"
            // Item4 Explanation
            var keyValues = new List<Tuple<string, Predicate<string>, string, string>>
            {
                new Tuple<string, Predicate<string>, string, string>(
                    "CMSRenewSessionAuthChange",
                    val => val == null || val.ToString().ToLower() == "true", 
                    "Session fixation (<add key=\"CMSRenewSessionAuthChange\" ...)",
                    "Consider enabling session renewal, to enforce User session disposal: https://docs.kentico.com/k12/securing-websites/designing-secure-websites/securing-and-protecting-the-system/session-protection"
                    ),
                new Tuple<string, Predicate<string>, string, string>(
                    "CMSEnableCsrfProtection",
                    val => val == null || val.ToString().ToLower() == "true",
                    "CSRF protection (<add key=\"CMSEnableCsrfProtection\" ...)",
                    "Default CSRF disabled. Ensure custom protection has been implemented: https://docs.kentico.com/k12/securing-websites/developing-secure-websites/cross-site-request-forgery-csrf-xsrf#Crosssiterequestforgery(CSRF/XSRF)-AvoidingCSRF"
                    ),
                new Tuple<string, Predicate<string>, string, string>(
                    "CMSHashStringSalt",
                    val => val != null,
                    "Hash string salt (<add key=\"CMSHashStringSalt\" ...)",
                    "Macro signature hash salt not set. This may cause macro security to break if CMSConnectionString is changed. Generate a GUID value: https://docs.kentico.com/k12/macro-expressions/troubleshooting-macros/working-with-macro-signatures"
                    ),
                new Tuple<string, Predicate<string>, string, string>(
                    "CMSXFrameOptionsExcluded",
                    val => string.IsNullOrEmpty(val),
                    "Click jacking protection (<add key=\"CMSXFrameOptionsExcluded\"...)",
                    "Click jacking protection is disabled for these paths. See documentation: https://docs.kentico.com/k12/securing-websites/designing-secure-websites/securing-and-protecting-the-system/clickjacking-protection"
                )
            };
            
            foreach (var key in keyValues)
            {
                // If value does not match expected condition, add row to result.
                var val = configuration.AppSettings.Settings[key.Item1]?.Value;
                if (!key.Item2(val))
                {
                    result.Rows.Add(key.Item3, val, key.Item4);
                }
            }

            #endregion

            // Return result depending on findings
            if (result.Rows.Count > 0)
            {
                return new ModuleResults
                {
                    Result = result,
                    ResultComment = "This settings must be fixed to ensure security requirements.",
                    Status = Status.Error,
                };
            }
            else
            {
                return new ModuleResults
                {
                    ResultComment = "Web.config settings look good.",
                    Status = Status.Good
                };
            }
        }

        # region "Protected methods"

        /// <summary>
        /// Validates Kentico instance path.
        /// Does nothing when <paramref name="validationErrorMessage"/> already contains a message.
        /// </summary>
        /// <param name="path">Path to Kentico instance.</param>
        /// <param name="validationErrorMessage">Message describing the cause of validation failure (set only when validation fails).</param>
        /// <returns>True if validation succeeds, false otherwise.</returns>
        protected bool ValidateInstancePath(string path, ref string validationErrorMessage)
        {
            if (!Directory.Exists(path))
            {
                validationErrorMessage = $"The given path '{path}' to Kentico instance is not valid. Maybe it is not accessible by the tool.";

                return false;
            }

            return true;
        }


        /// <summary>
        /// Gets error response.
        /// </summary>
        /// <param name="errorMessage">Error message.</param>
        /// <returns>Module results containing error message.</returns>
        protected ModuleResults GetErrorResponse(string errorMessage)
        {
            return new ModuleResults
            {
                Result = errorMessage,
                Status = Status.Error
            };
        }

        # endregion
    }
}

using System;
using System.Configuration;
using System.Data;
using System.IO;
using System.Web;
using System.Web.Configuration;
using Kentico.KInspector.Core;

namespace Kentico.KInspector.Modules
{
    public class SecurityAppSettingsModule : IModule
    {
        private const string RECOMMENDED_VALUE_TRUE = "True";
        private const string RECOMMENDED_VALUE_FALSE = "False";
        private const string VALUE_NOT_SET = "Settings was not set at all.";

        public ModuleMetadata GetModuleMetadata()
        {
            return new ModuleMetadata
            {
                Name = "Security web.config settings",
                Comment = "Checks security settings in web.config.",
                SupportedVersions = new[] { 
                    new Version("7.0"),
                    new Version("8.0"), 
                    new Version("8.1"), 
                    new Version("8.2"),
                    new Version("9.0")
                },
                Category = "Security",
            };
        }

        public ModuleResults GetResults(InstanceInfo instanceInfo)
        {
            // Prepare result
            DataTable result = new DataTable();
            result.Columns.Add("Key", typeof(string));
            result.Columns.Add("Actual value", typeof(string));
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
            var compilationNode = (CompilationSection) configuration.GetSection("system.web/compilation");
            bool debugMode = compilationNode.Debug;

            if (debugMode)
            {                
                result.Rows.Add("Debug (<compilation debug=\"...)", debugMode.ToString(), RECOMMENDED_VALUE_FALSE);
            }

            # endregion

            # region "Tracing"

            var traceNode = (TraceSection) configuration.GetSection("system.web/trace");
            bool tracing = traceNode.Enabled;

            if (tracing)
            {
                result.Rows.Add("Tracing (<trace enabled=\"...)", tracing.ToString(), RECOMMENDED_VALUE_FALSE);
            }

            # endregion

            # region "Custom errors"

            var customErrorsNode = (CustomErrorsSection) configuration.GetSection("system.web/customErrors");
            var customErrors = customErrorsNode.Mode;

            if (customErrors != CustomErrorsMode.On)
            {
                result.Rows.Add("Custom errors (<customErrors mode=\"...)", customErrors.ToString(), CustomErrorsMode.On.ToString());
            }

            # endregion

            # region "Cookieless authentication"

            var authNode = (AuthenticationSection) configuration.GetSection("system.web/authentication");
            var cookieless = authNode.Forms.Cookieless;

            if (cookieless != HttpCookieMode.UseCookies) // Auto? Device?
            {
                result.Rows.Add("Cookieless auth (<forms cookieless=\"...)", cookieless.ToString(), HttpCookieMode.UseCookies.ToString());
            }

            # endregion

            # region "Session fixation"

            // Check web.config keys (according to Kentico security audit POI)
            string sessionFixation = VALUE_NOT_SET;
            bool sessionFixationEnabled = false;

            try
            {
                sessionFixation = configuration.AppSettings.Settings["CMSRenewSessionAuthChange"].Value;
            }
            catch (Exception ex)
            {
                // Do nothing, value is not set.
            }

            if (!(Boolean.TryParse(sessionFixation, out sessionFixationEnabled) && sessionFixationEnabled))
            {
                result.Rows.Add("Session fixation (<add key=\"CMSRenewSessionAuthChange\" ...)", sessionFixation, RECOMMENDED_VALUE_TRUE);
            }

            # endregion

            # region "HttpOnlyCookies"

            var httpOnlyCookiesNode = (HttpCookiesSection) configuration.GetSection("system.web/httpCookies");
            bool httpOnlyCookies = httpOnlyCookiesNode.HttpOnlyCookies;

            if (!httpOnlyCookies) 
            {
                result.Rows.Add("HttpOnlyCookies (<httpCookies httpOnlyCookies=\"...)", httpOnlyCookies.ToString(), RECOMMENDED_VALUE_TRUE);
            }

            # endregion

            # region "Viewstate (MAC) validation"

            var pagesNode = (PagesSection) configuration.GetSection("system.web/pages");
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
            
            # endregion

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
                validationErrorMessage = String.Format("The given path '{0}' to Kentico instance is not valid. Maybe it is not accessible by the tool.", path);

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

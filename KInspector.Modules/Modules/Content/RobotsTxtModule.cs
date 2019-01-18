using System;
using System.Net;
using Kentico.KInspector.Core;

namespace Kentico.KInspector.Modules
{
    public class RobotsTxtModule: IModule
    {
        public ModuleMetadata GetModuleMetadata()
        {
            return new ModuleMetadata
            { 
                Name = "Robots.txt", 
                SupportedVersions = new[] { 
                    new Version("6.0"),
                    new Version("7.0"),
                    new Version("8.0"), 
                    new Version("8.1"), 
                    new Version("8.2"),
                    new Version("9.0"), 
                    new Version("10.0"), 
                    new Version("11.0"), 
                    new Version("12.0")
                },
                Comment = @"Checks that the ~/robots.txt file is present and accessible. See http://www.robotstxt.org/robotstxt.html for more details",
                
                Category = "Content",
            };
        }

        public ModuleResults GetResults(IInstanceInfo instanceInfo)
        {
            if (!TestUrl(instanceInfo.Uri, "robots.txt"))
            {
                return new ModuleResults
                {
                    Status = Status.Warning,
                    Result = "Robots.txt does not exist.",
                };
            }

            return new ModuleResults
            {
                Status = Status.Good,
                Result = "Robots.txt exists or is inaccessible.",
            };
        }

        private static bool TestUrl(Uri url, string file)
        {
            try
            {
                HttpWebRequest request = WebRequest.CreateHttp(new Uri(url, file));
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    return response.StatusCode == HttpStatusCode.OK;
                }
            }
            catch
            {
                return false;
            }

        }
    }
}

using System;
using System.Data;
using System.Net;
using Kentico.KInspector.Core;

namespace Kentico.KInspector.Modules
{
    public class CacheItemsModule : IModule
    {
        public ModuleMetadata GetModuleMetadata()
        {
            return new ModuleMetadata
            {
                Name = "Cache items",
                SupportedVersions = new[] { 
                    new Version("8.0"), 
                    new Version("8.1"), 
                    new Version("8.2"),
                    new Version("9.0")
                },
                Comment = @"Shows cache items and their size.",
            };
        }


        public ModuleResults GetResults(InstanceInfo instanceInfo)
        {
            try
            {
                ProbeHelper.InstallProbe(instanceInfo.Directory);

                var uri = new Uri(instanceInfo.Uri, "CMSPages/KInspectorProbe.aspx");
                HttpWebRequest request = WebRequest.CreateHttp(uri);
                using (WebResponse response = request.GetResponse())
                {
                    DataTable result = new DataTable();
                    result.ReadXml(response.GetResponseStream());

                    return new ModuleResults
                    {
                        Result = result,
                    };
                }
            }
            catch (Exception e)
            {
                // Probably 404
                return new ModuleResults
                {
                    Result = e.ToString(),
                    Status = Status.Error
                };
            }
            finally
            {
                ProbeHelper.UninstallProbe(instanceInfo.Directory);
            }
        }
    }
}

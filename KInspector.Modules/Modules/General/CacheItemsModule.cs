using System;
using System.Data;
using System.Net;
using KInspector.Core;
using KInspector.Modules.Helpers;

namespace KInspector.Modules.Modules.General
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
                    new Version("8.2") 
                },
                Comment = @"Shows cache items and their size.",
            };
        }


        public ModuleResults GetResults(InstanceInfo instanceInfo, DatabaseService dbService)
        {
            try
            {
                ProbeHelper.InstallProbe(instanceInfo.Directory);

                var uri = new Uri(instanceInfo.Url, "CMSPages/KInspectorProbe.aspx");
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

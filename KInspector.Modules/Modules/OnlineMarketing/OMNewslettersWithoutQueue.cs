using System;
using Kentico.KInspector.Core;

namespace Kentico.KInspector.Modules
{
    public class OMNewslettersWithoutQueue : IModule
    {
        public ModuleMetadata GetModuleMetadata()
        {
            return new ModuleMetadata
            {
                Name = "Newsletters not using email queue (chance of losing emails)",
                SupportedVersions = new[] { 
                    new Version("7.0"),
                    new Version("8.1"), 
                    new Version("8.2")
                },
                Comment = @"Displays the newsletters (email campaigns) that are not using email queue.

If newsletter is not sent via email queue, emails are generated into memory queue, which could be lost on server restart.
Default app pool recycle is every 29 hours. If you're sending a lot of emails, the probability is rather high.",
                Category = "Online marketing"
            };
        }

        public ModuleResults GetResults(InstanceInfo instanceInfo)
        {
            var dbService = instanceInfo.DBService;
            var newslettersWithDisabledQueue = dbService.ExecuteAndGetTableFromFile("OMNewslettersWithoutQueue.sql");
            if (newslettersWithDisabledQueue.Rows.Count > 0)
            {
                return new ModuleResults
                {
                    Result = newslettersWithDisabledQueue,
                    ResultComment = @"Enable email queue in all newsletters. (UPDATE Newsletter_Newsletter SET NewsletterUseEmailQueue = 1)",
                    Status = Status.Error,
                };
            }
            else
            {
                return new ModuleResults
                {
                    ResultComment = "All existing newsletters have email queue turned ON.",
                    Status = Status.Good
                };
            }
        }
    }
}

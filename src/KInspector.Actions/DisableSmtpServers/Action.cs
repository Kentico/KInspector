using KInspector.Actions.DisableSmtpServers.Models;
using KInspector.Core;
using KInspector.Core.Constants;
using KInspector.Core.Helpers;
using KInspector.Core.Models;
using KInspector.Core.Services.Interfaces;

namespace KInspector.Actions.DisableSmtpServers
{
    public class Action : AbstractAction<Terms, Options>
    {
        private readonly IDatabaseService databaseService;

        public override IList<Version> CompatibleVersions => VersionHelper.GetVersionList("12", "13");

        public override IList<string> Tags => new List<string> {
            ModuleTags.Configuration,
            ModuleTags.Emails
        };

        public Action(IDatabaseService databaseService, IModuleMetadataService moduleMetadataService) : base(moduleMetadataService)
        {
            this.databaseService = databaseService;
        }

        public override Task<ModuleResults> Execute(Options? options)
        {
            // Only partial options are allowed for this action
            throw new NotImplementedException();
        }

        public async override Task<ModuleResults> ExecutePartial(Options? options)
        {
            if (options?.ServerId is not null &&
                options?.ServerId > 0 &&
                options?.SiteId is null)
            {
                var serversFromSmtp = await databaseService.ExecuteSqlFromFile<SmtpFromSmtpServers>(Scripts.GetSmtpFromSmtpServers);
                if (!serversFromSmtp.Any(s => s.ID == options?.ServerId) ||
                    (!serversFromSmtp.FirstOrDefault(s => s.ID == options?.ServerId)?.Enabled ?? false))
                {
                    return await GetInvalidOptionsResult();
                }

                return await DisableServer(options?.ServerId);
            }

            if (options?.SiteId is not null &&
                options?.SiteId >= 0 &&
                options?.ServerId is null)
            {
                var serversFromSettings = await databaseService.ExecuteSqlFromFile<SmtpFromSettings>(Scripts.GetSmtpFromSettingsKeys);
                if (!serversFromSettings.Any(s => s.SiteID == options?.SiteId) ||
                    serversFromSettings.FirstOrDefault(s => s.SiteID == options?.SiteId)?.Server is null ||
                    (serversFromSettings.FirstOrDefault(s => s.SiteID == options?.SiteId)?.Server?.EndsWith(".disabled") ?? false))
                {
                    return await GetInvalidOptionsResult();
                }

                return await DisableSiteSetting(options?.SiteId);
            }

            return await GetInvalidOptionsResult();
        }

        public async override Task<ModuleResults> ExecuteListing()
        {
            var serversFromSettings = await databaseService.ExecuteSqlFromFile<SmtpFromSettings>(Scripts.GetSmtpFromSettingsKeys);
            var serversFromSmtp = await databaseService.ExecuteSqlFromFile<SmtpFromSmtpServers>(Scripts.GetSmtpFromSmtpServers);
            var results = new ModuleResults
            {
                Type = ResultsType.TableList,
                Status = ResultsStatus.Information,
                Summary = Metadata.Terms.ListSummary
            };
            results.TableResults.Add(new TableResult
            {
                Name = Metadata.Terms.ServersFromSmtpTable,
                Rows = serversFromSmtp
            });
            results.TableResults.Add(new TableResult
            {
                Name = Metadata.Terms.ServersFromSettingsTable,
                Rows = serversFromSettings
            });

            return results;
        }

        public async override Task<ModuleResults> GetInvalidOptionsResult()
        {
            var result = await ExecuteListing();
            result.Status = ResultsStatus.Error;
            result.Summary = Metadata.Terms.InvalidOptions;

            return result;
        }

        private async Task<ModuleResults> DisableServer(int? serverId)
        {
            await databaseService.ExecuteNonQuery(Scripts.DisableSmtpServer, new { ServerID = serverId });
            var result = await ExecuteListing();
            result.Status = ResultsStatus.Good;
            result.Summary = Metadata.Terms.ServerDisabled?.With(new
            {
                serverId
            });

            return result;
        }

        private async Task<ModuleResults> DisableSiteSetting(int? siteId)
        {
            await databaseService.ExecuteNonQuery(Scripts.DisableSiteSmtpServer, new { SiteID = siteId });
            var result = await ExecuteListing();
            result.Status = ResultsStatus.Good;
            result.Summary = Metadata.Terms.SiteSettingDisabled?.With(new
            {
                siteId
            });

            return result;
        }
    }
}
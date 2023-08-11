using KenticoInspector.Actions.SmtpServerSummary.Models;
using KenticoInspector.Core;
using KenticoInspector.Core.Constants;
using KenticoInspector.Core.Helpers;
using KenticoInspector.Core.Models;
using KenticoInspector.Core.Services.Interfaces;

using System;
using System.Collections.Generic;
using System.Linq;

namespace KenticoInspector.Actions.SmtpServerSummary
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

        public override ActionResults Execute(Options options)
        {
            // Only partial options are allowed for this action
            throw new NotImplementedException();
        }

        public override ActionResults ExecutePartial(Options options)
        {
            if (options.ServerId != null &&
                options.ServerId > 0 &&
                options.SiteId == null)
            {
                var serversFromSmtp = databaseService.ExecuteSqlFromFile<SmtpFromSmtpServers>(Scripts.GetSmtpFromSmtpServers);
                if (!serversFromSmtp.Any(s => s.ID == options.ServerId) ||
                    !serversFromSmtp.FirstOrDefault(s => s.ID == options.ServerId).Enabled)
                {
                    return GetInvalidOptionsResult();
                }

                return DisableServer(options.ServerId);
            }

            if (options.SiteId != null &&
                options.SiteId >= 0 &&
                options.ServerId == null)
            {
                var serversFromSettings = databaseService.ExecuteSqlFromFile<SmtpFromSettings>(Scripts.GetSmtpFromSettingsKeys);
                if (!serversFromSettings.Any(s => s.SiteID == options.SiteId) ||
                    serversFromSettings.FirstOrDefault(s => s.SiteID == options.SiteId).Server.EndsWith(".disabled"))
                {
                    return GetInvalidOptionsResult();
                }

                return DisableSiteSetting(options.SiteId);
            }

            return GetInvalidOptionsResult();
        }

        public override ActionResults ExecuteListing()
        {
            var serversFromSettings = databaseService.ExecuteSqlFromFile<SmtpFromSettings>(Scripts.GetSmtpFromSettingsKeys);
            var settingsTable = new TableResult<SmtpFromSettings>()
            {
                Name = Metadata.Terms.ServersFromSettingsTable,
                Rows = serversFromSettings
            };

            var serversFromSmtp = databaseService.ExecuteSqlFromFile<SmtpFromSmtpServers>(Scripts.GetSmtpFromSmtpServers);
            var smtpTable = new TableResult<SmtpFromSmtpServers>()
            {
                Name = Metadata.Terms.ServersFromSmtpTable,
                Rows = serversFromSmtp
            };

            var results = new ActionResults
            {
                Type = ResultsType.TableList,
                Status = ResultsStatus.Information,
                Summary = Metadata.Terms.ListSummary
            };

            results.Data.SettingsTable = settingsTable;
            results.Data.SmtpTable = smtpTable;

            return results;
        }

        public override ActionResults GetInvalidOptionsResult()
        {
            var result = ExecuteListing();
            result.Status = ResultsStatus.Error;
            result.Summary = Metadata.Terms.InvalidOptions;

            return result;
        }

        private ActionResults DisableServer(int? serverId)
        {
            databaseService.ExecuteSqlFromFileGeneric(Scripts.DisableSmtpServer, new { ServerID = serverId });
            var result = ExecuteListing();
            result.Status = ResultsStatus.Good;
            result.Summary = Metadata.Terms.ServerDisabled.With(new
            {
                serverId
            });

            return result;
        }

        private ActionResults DisableSiteSetting(int? siteId)
        {
            databaseService.ExecuteSqlFromFileGeneric(Scripts.DisableSiteSmtpServer, new { SiteID = siteId });
            var result = ExecuteListing();
            result.Status = ResultsStatus.Good;
            result.Summary = Metadata.Terms.SiteSettingDisabled.With(new
            {
                siteId
            });

            return result;
        }
    }
}
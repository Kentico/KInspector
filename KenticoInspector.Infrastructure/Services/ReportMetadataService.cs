using System;
using System.IO;
using System.Threading;

using KenticoInspector.Core.Models;
using KenticoInspector.Core.Services.Interfaces;

using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace KenticoInspector.Core.Helpers
{
    public class ReportMetadataService : IReportMetadataService
    {
        private readonly IInstanceService instanceService;

        public string CurrentCultureName => Thread.CurrentThread.CurrentCulture.Name;

        public ReportMetadataService(IInstanceService instanceService)
        {
            this.instanceService = instanceService;
        }

        public ReportMetadata<TTerms> GetReportMetadata<TTerms>(string reportCodename) where TTerms : new()
        {
            var yamlPath = $"{DirectoryHelper.GetExecutingDirectory()}\\{reportCodename}\\Metadata\\{CurrentCultureName}.yaml";

            var deserializedMetadata = DeserializeYaml<ReportMetadata<TTerms>>(yamlPath);

            var instanceDetails = instanceService.GetInstanceDetails(instanceService.CurrentInstance);

            var commonData = new
            {
                instanceUrl = instanceService.CurrentInstance.Url,
                administrationVersion = instanceDetails.AdministrationVersion,
                databaseVersion = instanceDetails.DatabaseVersion
            };

            deserializedMetadata.Details.Name = deserializedMetadata.Details.Name.With(commonData);
            deserializedMetadata.Details.ShortDescription = deserializedMetadata.Details.ShortDescription.With(commonData);
            deserializedMetadata.Details.LongDescription = deserializedMetadata.Details.LongDescription.With(commonData);

            return deserializedMetadata;
        }

        public TMetadata DeserializeYaml<TMetadata>(string path)
        {
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(new CamelCaseNamingConvention())
                .Build();

            var yamlFile = File.ReadAllText(path);

            return deserializer.Deserialize<TMetadata>(yamlFile);
        }
    }
}
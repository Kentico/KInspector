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
        public string DefaultCultureName => "en-US";

        public string CurrentCultureName => Thread.CurrentThread.CurrentCulture.Name;

        public ReportMetadata<T> GetReportMetadata<T>(string reportCodename)
            where T : new()
        {
            var metadataDirectory = $"{DirectoryHelper.GetExecutingDirectory()}\\{reportCodename}\\Metadata\\";

            var reportMetadata = DeserializeMetadataFromYamlFile<ReportMetadata<T>>(
                metadataDirectory,
                CurrentCultureName,
                false
            );

            var currentCultureIsDefaultCulture = CurrentCultureName == DefaultCultureName;

            if (!currentCultureIsDefaultCulture)
            {
                var defaultReportMetadata = DeserializeMetadataFromYamlFile<ReportMetadata<T>>(
                    metadataDirectory,
                    DefaultCultureName,
                    true
                );

                return GetMergedMetadata(defaultReportMetadata, reportMetadata);
            }
            else
            {
                return reportMetadata;
            }
        }

        private static T DeserializeMetadataFromYamlFile<T>(
            string metadataDirectory,
            string cultureName,
            bool ignoreUnmatchedProperties)
            where T : new()
        {
            var reportMetadataPath = $"{metadataDirectory}{cultureName}.yaml";

            var reportMetadataPathExists = File.Exists(reportMetadataPath);

            if (reportMetadataPathExists)
            {
                var fileText = File.ReadAllText(reportMetadataPath);

                return DeserializeYaml<T>(fileText, ignoreUnmatchedProperties);
            }

            return new T();
        }

        private static T DeserializeYaml<T>(
            string yaml,
            bool ignoreUnmatchedProperties)
        {
            var deserializerBuilder = new DeserializerBuilder()
                .WithNamingConvention(new CamelCaseNamingConvention());

            if (ignoreUnmatchedProperties)
            {
                deserializerBuilder.IgnoreUnmatchedProperties();
            }

            var deserializer = deserializerBuilder.Build();

            return deserializer.Deserialize<T>(yaml);
        }

        private static ReportMetadata<T> GetMergedMetadata<T>(
            ReportMetadata<T> defaultMetadata,
            ReportMetadata<T> overrideMetadata)
            where T : new()
        {
            var mergedMetadata = new ReportMetadata<T>();

            mergedMetadata.Details.Name = overrideMetadata.Details.Name ?? defaultMetadata.Details.Name;
            mergedMetadata.Details.ShortDescription = overrideMetadata.Details.ShortDescription ?? defaultMetadata.Details.ShortDescription;
            mergedMetadata.Details.LongDescription = overrideMetadata.Details.LongDescription ?? defaultMetadata.Details.LongDescription;

            RecursivelySetPropertyValues(
                typeof(T),
                defaultMetadata.Terms,
                overrideMetadata.Terms,
                mergedMetadata.Terms);

            return mergedMetadata;
        }

        private static void RecursivelySetPropertyValues(
            Type objectType,
            object defaultObject,
            object overrideObject,
            object targetObject)
        {
            var objectTypeProperties = objectType.GetProperties();

            foreach (var objectTypeProperty in objectTypeProperties)
            {
                var objectTypePropertyType = objectTypeProperty.PropertyType;

                var defaultObjectPropertyValue = objectTypeProperty.GetValue(defaultObject);

                object overrideObjectPropertyValue = overrideObject != null
                    ? objectTypeProperty.GetValue(overrideObject)
                    : defaultObjectPropertyValue;

                if (objectTypePropertyType.Namespace == objectType.Namespace)
                {
                    var targetObjectPropertyValue = Activator.CreateInstance(
                        objectTypePropertyType);

                    objectTypeProperty.SetValue(
                        targetObject,
                        targetObjectPropertyValue);

                    RecursivelySetPropertyValues(
                        objectTypePropertyType,
                        defaultObjectPropertyValue,
                        overrideObjectPropertyValue,
                        targetObjectPropertyValue);
                }
                else
                {
                    objectTypeProperty.SetValue(
                        targetObject,
                        overrideObjectPropertyValue);
                }
            }
        }
    }
}
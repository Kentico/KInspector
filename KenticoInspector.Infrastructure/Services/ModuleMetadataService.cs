using System;
using System.IO;
using System.Threading;

using KenticoInspector.Core.Models;
using KenticoInspector.Core.Services.Interfaces;

using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace KenticoInspector.Core.Helpers
{
    public class ModuleMetadataService : IModuleMetadataService
    {
        private readonly IInstanceService instanceService;

        public string DefaultCultureName => "en-US";

        public string CurrentCultureName => Thread.CurrentThread.CurrentCulture.Name;

        public ModuleMetadataService(IInstanceService instanceService)
        {
            this.instanceService = instanceService;
        }

        public ModuleMetadata<T> GetModuleMetadata<T>(string moduleCodename)
            where T : new()
        {
            var metadataDirectory = $"{DirectoryHelper.GetExecutingDirectory()}\\{moduleCodename}\\Metadata\\";

            var currentMetadata = DeserializeMetadataFromYamlFile<ModuleMetadata<T>>(
                metadataDirectory,
                CurrentCultureName,
                false
            );

            var currentCultureIsDefaultCulture = CurrentCultureName == DefaultCultureName;

            var mergedMetadata = new ModuleMetadata<T>();

            if (!currentCultureIsDefaultCulture)
            {
                var defaultMetadata = DeserializeMetadataFromYamlFile<ModuleMetadata<T>>(
                    metadataDirectory,
                    DefaultCultureName,
                    true
                );

                mergedMetadata = GetMergedMetadata(defaultMetadata, currentMetadata);
            }

            var ModuleMetadata = currentCultureIsDefaultCulture ? currentMetadata : mergedMetadata;

            var instanceDetails = instanceService.GetInstanceDetails(instanceService.CurrentInstance);

            var commonData = new
            {
                instanceUrl = instanceService.CurrentInstance.AdminUrl,
                administrationVersion = instanceDetails.AdministrationVersion,
                databaseVersion = instanceDetails.DatabaseVersion
            };

            Term name = ModuleMetadata.Details.Name;

            ModuleMetadata.Details.Name = name.With(commonData);

            Term shortDescription = ModuleMetadata.Details.ShortDescription;

            ModuleMetadata.Details.ShortDescription = shortDescription.With(commonData);

            Term longDescription = ModuleMetadata.Details.LongDescription;

            ModuleMetadata.Details.LongDescription = longDescription.With(commonData);

            return ModuleMetadata;
        }

        private static T DeserializeMetadataFromYamlFile<T>(
            string metadataDirectory,
            string cultureName,
            bool ignoreUnmatchedProperties)
            where T : new()
        {
            var ModuleMetadataPath = $"{metadataDirectory}{cultureName}.yaml";

            var ModuleMetadataPathExists = File.Exists(ModuleMetadataPath);

            if (ModuleMetadataPathExists)
            {
                var fileText = File.ReadAllText(ModuleMetadataPath);

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

        private static ModuleMetadata<T> GetMergedMetadata<T>(
            ModuleMetadata<T> defaultMetadata,
            ModuleMetadata<T> overrideMetadata)
            where T : new()
        {
            var mergedMetadata = new ModuleMetadata<T>();

            mergedMetadata.Details.Name = overrideMetadata.Details.Name ?? defaultMetadata.Details.Name;
            mergedMetadata.Details.ShortDescription = 
                overrideMetadata.Details.ShortDescription ?? defaultMetadata.Details.ShortDescription;
            mergedMetadata.Details.LongDescription = 
                overrideMetadata.Details.LongDescription ?? defaultMetadata.Details.LongDescription;

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
                    var targetObjectPropertyValue = Activator.CreateInstance(objectTypePropertyType);

                    objectTypeProperty.SetValue(targetObject, targetObjectPropertyValue);

                    RecursivelySetPropertyValues(
                        objectTypePropertyType,
                        defaultObjectPropertyValue,
                        overrideObjectPropertyValue,
                        targetObjectPropertyValue);
                }
                else
                {
                    objectTypeProperty.SetValue(targetObject, overrideObjectPropertyValue);
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using KenticoInspector.Core.Models;
using Newtonsoft.Json;

namespace KenticoInspector.Core.Services
{
    public class FileSystemInstanceConfigurationService : IInstanceConfigurationService
    {
        private string saveFileLocation = $"{Directory.GetCurrentDirectory()}\\InstanceConfigurations.json";

        public void Delete(Guid Guid)
        {
            var configurations = LoadConfigurations();
            var configurationIndex = configurations.FindIndex(i => i.Guid == Guid);
            configurations.RemoveAt(configurationIndex);
            SaveConfigurations(configurations);
        }

        public InstanceConfiguration GetItem(Guid guid)
        {
            var items = GetItems();
            return items.Exists(i=> i.Guid == guid) ? items.First(i=> i.Guid == guid) : null;
        }

        public List<InstanceConfiguration> GetItems()
        {
            return LoadConfigurations();
        }

        public Guid Upsert(InstanceConfiguration instanceConfiguration)
        {
            instanceConfiguration.Guid = instanceConfiguration.Guid == Guid.Empty ? Guid.NewGuid() : instanceConfiguration.Guid;

            var configurations = LoadConfigurations();

            var existingConfigurationIndex = configurations.FindIndex(x => x.Guid == instanceConfiguration.Guid);

            if (existingConfigurationIndex == -1)
            {
                configurations.Add(instanceConfiguration);
            }
            else
            {
                configurations[existingConfigurationIndex] = instanceConfiguration;
            }
            
            SaveConfigurations(configurations);

            return instanceConfiguration.Guid;
        }

        private List<InstanceConfiguration> LoadConfigurations()
        {
            List<InstanceConfiguration> configurations = null;

            var saveFileExists = File.Exists(saveFileLocation);            
            if (saveFileExists)
            {
                var saveFileContents = File.ReadAllText(saveFileLocation);
                configurations= JsonConvert.DeserializeObject<List<InstanceConfiguration>>(saveFileContents);
            }

            return configurations ?? new List<InstanceConfiguration>();
        }

        private void SaveConfigurations(List<InstanceConfiguration> configurations)
        {
            var configurationsJson = JsonConvert.SerializeObject(configurations, Formatting.Indented);
            File.WriteAllText(saveFileLocation, configurationsJson);
        }
    }
}

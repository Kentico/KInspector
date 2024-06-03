using KInspector.Core.Models;
using KInspector.Core.Services.Interfaces;

using Newtonsoft.Json;

namespace KInspector.Infrastructure.Services
{
    public class ConfigService : IConfigService
    {
        private readonly string _saveFileLocation = $"{Directory.GetCurrentDirectory()}\\KInspector.config";

        public bool DeleteInstance(Guid? guid)
        {
            var config = GetConfig();
            var totalRemoved = config.Instances.RemoveAll(i => i.Guid.Equals(guid));
            SaveConfig(config);

            return totalRemoved > 0;
        }

        public Instance? GetInstance(Guid guid)
        {
            var config = GetConfig();

            return config.Instances.FirstOrDefault(i => i.Guid.Equals(guid));
        }

        public InspectorConfig GetConfig()
        {
            if (File.Exists(_saveFileLocation))
            {
                var saveFileContents = File.ReadAllText(_saveFileLocation);
                var config = JsonConvert.DeserializeObject<InspectorConfig>(saveFileContents);

                return config ?? new InspectorConfig();
            }

            var newConfig = new InspectorConfig();
            SaveConfig(newConfig);

            return newConfig;
        }

        public Instance? GetCurrentInstance()
        {
            var config = GetConfig();

            return config.Instances.FirstOrDefault(i => i.Guid == config.CurrentInstance);
        }

        public Instance? SetCurrentInstance(Guid? guid)
        {
            var config = GetConfig();
            var selectedInstance = config.Instances.FirstOrDefault(i => i.Guid.Equals(guid));
            config.CurrentInstance = guid;
            SaveConfig(config);

            return selectedInstance;
        }

        public void UpsertInstance(Instance instance)
        {
            instance.Guid = instance.Guid == Guid.Empty ? Guid.NewGuid() : instance.Guid;
            var config = GetConfig();
            var existingSettingsIndex = config.Instances.FindIndex(x => x.Guid.Equals(instance.Guid));
            if (existingSettingsIndex == -1)
            {
                config.Instances.Add(instance);
            }
            else
            {
                config.Instances[existingSettingsIndex] = instance;
            }

            SaveConfig(config);
        }

        private void SaveConfig(InspectorConfig config)
        {
            var jsonText = JsonConvert.SerializeObject(config, Formatting.Indented);
            File.WriteAllText(_saveFileLocation, jsonText);
        }
    }
}
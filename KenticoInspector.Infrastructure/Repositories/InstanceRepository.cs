using KenticoInspector.Core.Models;
using KenticoInspector.Core.Repositories.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace KenticoInspector.Infrastructure.Repositories
{
    public class InstanceRepository : IInstanceRepository
    {
        private readonly string _saveFileLocation = $"{Directory.GetCurrentDirectory()}\\SavedInstances.json";

        public bool DeleteInstance(Guid guid)
        {
            var currentInstances = LoadSavedInstances();
            var totalRemoved = currentInstances.RemoveAll(i => i.Guid == guid);
            SaveInstances(currentInstances);
            return totalRemoved > 0;
        }

        public Instance GetInstance(Guid guid)
        {
            var instances = GetInstances();
            var selectedInstance = instances.FirstOrDefault(i => i.Guid == guid);

            return selectedInstance;
        }

        public IList<Instance> GetInstances()
        {
            return LoadSavedInstances();
        }

        public Instance UpsertInstance(Instance instance)
        {
            instance.Guid = instance.Guid == Guid.Empty ? Guid.NewGuid() : instance.Guid;

            var savedInstanceSettings = LoadSavedInstances();
            var existingSettingsIndex = savedInstanceSettings.FindIndex(x => x.Guid == instance.Guid);

            if (existingSettingsIndex == -1)
            {
                savedInstanceSettings.Add(instance);
            }
            else
            {
                savedInstanceSettings[existingSettingsIndex] = instance;
            }

            SaveInstances(savedInstanceSettings);

            return instance;
        }

        private List<Instance> LoadSavedInstances()
        {
            var saveFileExists = File.Exists(_saveFileLocation);
            if (saveFileExists)
            {
                var saveFileContents = File.ReadAllText(_saveFileLocation);
                var loadedInstances = JsonConvert.DeserializeObject<List<Instance>>(saveFileContents);

                return loadedInstances;
            }

            return new List<Instance>();
        }

        private void SaveInstances(List<Instance> instance)
        {
            var jsonText = JsonConvert.SerializeObject(instance, Formatting.Indented);
            File.WriteAllText(_saveFileLocation, jsonText);
        }
    }
}
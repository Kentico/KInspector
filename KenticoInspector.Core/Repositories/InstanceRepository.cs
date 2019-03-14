using Dapper;
using KenticoInspector.Core.Helpers;
using KenticoInspector.Core.Models;
using KenticoInspector.Core.Repositories.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace KenticoInspector.Core.Repositories
{
    public class InstanceRepository : IInstanceRepository
    {
        private readonly string _saveFileLocation = $"{Directory.GetCurrentDirectory()}\\SavedInstances.json";
                
        public bool DeleteInstance(Guid guid)
        {
            return DeleteInstanceInternal(guid);
        }

        public Instance GetInstance(Guid guid)
        {
            return GetInstanceInternal(guid);
        }

        public List<Instance> GetInstances()
        {
            return LoadSavedInstances(true);
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

        private bool DeleteInstanceInternal(Guid guid)
        {
            var currentInstances = LoadSavedInstances();
            var totalRemoved = currentInstances.RemoveAll(i => i.Guid == guid);
            SaveInstances(currentInstances);
            return totalRemoved > 0;
        }

        private Instance GetInstanceInternal(Guid guid)
        {
            var instances = GetInstances();
            var selectedInstance = instances.FirstOrDefault(i => i.Guid == guid);
            if (selectedInstance != null) {
                LoadInstanceDynamicProperties(selectedInstance);
            }

            return selectedInstance;
        }

        private List<Site> GetInstanceSites(Instance instance)
        {
            // TODO: Get sites for instance
            return new List<Site>();
        }

        private Version GetKenticoAdministrationVersion(Instance instance)
        {
            try
            {
                var instanceConnection = DatabaseHelper.GetSqlConnection(instance.DatabaseSettings);

                using (var connection = instanceConnection)
                {
                    var query = "SELECT KeyValue FROM CMS_SettingsKey WHERE KeyName = 'CMSDBVersion'";
                    connection.Open();
                    var version = connection.QuerySingle<string>(query);
                    return new Version(version);
                }
            }
            catch (Exception e)
            {
                instance.AddErrorMessage("Could not retrieve database version", e);
                return null;
            }            
        }

        private void LoadInstanceDynamicProperties(Instance instance)
        {
            if (instance != null)
            {
                // TODO: Get administration version from disk
                instance.KenticoAdministrationVersion = new Version();
                instance.KenticoDatabaseVersion = GetKenticoAdministrationVersion(instance);
                instance.Sites = GetInstanceSites(instance);
            }
        }

        private List<Instance> LoadSavedInstances(bool loadDynamicProperties = false)
        {
            var saveFileExists = File.Exists(_saveFileLocation);
            if (saveFileExists)
            {
                var saveFileContents = File.ReadAllText(_saveFileLocation);
                var loadedInstances = JsonConvert.DeserializeObject<List<Instance>>(saveFileContents);
                if (loadDynamicProperties) {
                    foreach (var instance in loadedInstances)
                    {
                        LoadInstanceDynamicProperties(instance);
                    }
                }

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

using System;
using System.Collections.Generic;
using System.Linq;

using KenticoInspector.Core.Models;
using KenticoInspector.Core.Repositories.Interfaces;
using KenticoInspector.Core.Services.Interfaces;

namespace KenticoInspector.Infrastructure.Services
{
    public class InstanceService : IInstanceService
    {
        private readonly IInstanceRepository _instanceRepository;
        private readonly ISiteRepository _siteRepository;
        private readonly IVersionRepository _versionRepository;

        public Instance CurrentInstance { get; private set; }

        public InstanceService(IInstanceRepository instanceRepository, IVersionRepository versionRepository, ISiteRepository siteRepository)
        {
            _instanceRepository = instanceRepository;
            _versionRepository = versionRepository;
            _siteRepository = siteRepository;
        }

        public bool DeleteInstance(Guid instanceGuid)
        {
            return _instanceRepository.DeleteInstance(instanceGuid);
        }

        public Instance GetInstance(Guid instanceGuid)
        {
            return _instanceRepository.GetInstance(instanceGuid);
        }

        public Instance SetCurrentInstance(Guid instanceGuid)
        {
            CurrentInstance = _instanceRepository.GetInstance(instanceGuid);

            return CurrentInstance;
        }

        public InstanceDetails GetInstanceDetails(Guid instanceGuid)
        {
            var instance = _instanceRepository.GetInstance(instanceGuid);
            return GetInstanceDetails(instance);
        }

        public InstanceDetails GetInstanceDetails(Instance instance)
        {
            return new InstanceDetails
            {
                Guid = instance.Guid,
                AdministrationVersion = _versionRepository.GetKenticoAdministrationVersion(instance),
                DatabaseVersion = _versionRepository.GetKenticoDatabaseVersion(instance),
                Sites = _siteRepository.GetSites(instance).ToList()
            };
        }

        public IList<Instance> GetInstances()
        {
            return _instanceRepository.GetInstances();
        }

        public Instance UpsertInstance(Instance instance)
        {
            return _instanceRepository.UpsertInstance(instance);
        }
    }
}
using KenticoInspector.Core.Models;
using KenticoInspector.Core.Repositories.Interfaces;
using KenticoInspector.Core.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KenticoInspector.Infrastructure.Services
{
    public class InstanceService : IInstanceService
    {
        private readonly IInstanceRepository _instanceRepository;
        private readonly ISiteRepository _siteRepository;
        private readonly IVersionRepository _versionRepository;

        public InstanceService(IInstanceRepository instanceRepository, IVersionRepository versionRepository, ISiteRepository siteRepository)
        {
            _instanceRepository = instanceRepository;
            _versionRepository = versionRepository;
            _siteRepository = siteRepository;
        }

        public bool DeleteInstance(Guid guid)
        {
            return _instanceRepository.DeleteInstance(guid);
        }

        public Instance GetInstance(Guid guid)
        {
            return _instanceRepository.GetInstance(guid);
        }

        public InstanceDetails GetInstanceDetails(Guid guid)
        {
            var instance = _instanceRepository.GetInstance(guid);
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
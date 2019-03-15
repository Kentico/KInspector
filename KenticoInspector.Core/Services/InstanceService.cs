using KenticoInspector.Core.Models;
using KenticoInspector.Core.Repositories.Interfaces;
using KenticoInspector.Core.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace KenticoInspector.Core.Services
{
    public class InstanceService : IInstanceService
    {
        private readonly IVersionRepository _versionService;
        private readonly IInstanceRepository _instanceRepository;
        private readonly ISiteRepository _siteRepository;

        public InstanceService(IInstanceRepository instanceRepository, IVersionRepository versionService, ISiteRepository siteRepository)
        {
            _instanceRepository = instanceRepository;
            _versionService = versionService;
            _siteRepository = siteRepository;
        }

        public ConnectedInstanceDetails ConnectToInstance(Guid guid)
        {
            var instance = _instanceRepository.GetInstance(guid);
            return ConnectToInstance(instance);
        }

        public ConnectedInstanceDetails ConnectToInstance(Instance instance)
        {
            return new ConnectedInstanceDetails
            {
                Guid = instance.Guid,
                AdministrationVersion = _versionService.GetKenticoAdministrationVersion(instance),
                DatabaseVersion = _versionService.GetKenticoDatabaseVersion(instance),
                Sites = _siteRepository.GetSites(instance)
            };
        }

        public bool DeleteInstance(Guid guid)
        {
            return _instanceRepository.DeleteInstance(guid);
        }

        public Instance GetInstance(Guid guid)
        {
            return _instanceRepository.GetInstance(guid);
        }

        public List<Instance> GetInstances()
        {
            return _instanceRepository.GetInstances();
        }

        public Instance UpsertInstance(Instance instance)
        {
            return _instanceRepository.UpsertInstance(instance);
        }
    }
}

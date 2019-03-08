using KenticoInspector.Core.Models;
using System;
using System.Collections.Generic;

namespace KenticoInspector.Core.Services
{
    interface IInstanceConfigurationService
    {
        InstanceConfiguration GetItem(Guid Guid);

        List<InstanceConfiguration> GetItems();

        Guid Upsert(InstanceConfiguration instanceConfiguration);

        void Delete(Guid GUID);
    }
}

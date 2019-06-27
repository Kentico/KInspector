using KenticoInspector.Core.Models;
using System;
using System.Collections.Generic;

namespace KenticoInspector.Core.Services.Interfaces
{
    public interface IInstanceService : IService
    {
        bool DeleteInstance(Guid guid);

        Instance GetInstance(Guid guid);

        InstanceDetails GetInstanceDetails(Guid guid);

        InstanceDetails GetInstanceDetails(Instance instance);

        IList<Instance> GetInstances();

        Instance UpsertInstance(Instance instance);
    }
}
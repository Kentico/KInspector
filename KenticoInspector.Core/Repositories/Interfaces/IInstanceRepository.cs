using System;
using System.Collections.Generic;

using KenticoInspector.Core.Models;

namespace KenticoInspector.Core.Repositories.Interfaces
{
    public interface IInstanceRepository : IRepository
    {
        bool DeleteInstance(Guid guid);

        Instance GetInstance(Guid guid);

        IList<Instance> GetInstances();

        Instance UpsertInstance(Instance instance);
    }
}
using KenticoInspector.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace KenticoInspector.Core.Repositories.Interfaces
{
    public interface IInstanceRepository : IRepository
    {
        bool DeleteInstance(Guid guid);

        Instance GetInstance(Guid guid);

        List<Instance> GetInstances();

        Instance UpsertInstance(Instance instance);
    }
}

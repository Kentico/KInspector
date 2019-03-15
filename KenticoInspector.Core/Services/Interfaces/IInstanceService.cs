using KenticoInspector.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace KenticoInspector.Core.Services.Interfaces
{
    public interface IInstanceService : IService
    {
        ConnectedInstanceDetails ConnectToInstance(Guid guid);

        ConnectedInstanceDetails ConnectToInstance(Instance instance);

        bool DeleteInstance(Guid guid);

        Instance GetInstance(Guid guid);

        List<Instance> GetInstances();

        Instance UpsertInstance(Instance instance);
    }
}

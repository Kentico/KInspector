﻿using System;
using System.Collections.Generic;

using KenticoInspector.Core.Models;

namespace KenticoInspector.Core.Services.Interfaces
{
    public interface IInstanceService : IService
    {
        Instance CurrentInstance { get; }

        bool DeleteInstance(Guid instanceGuid);

        Instance GetInstance(Guid instanceGuid);

        InstanceDetails GetInstanceDetails(Guid instanceGuid);

        InstanceDetails GetInstanceDetails(Instance instance);

        IList<Instance> GetInstances();

        Instance SetCurrentInstance(Guid instanceGuid);

        Instance UpsertInstance(Instance instance);
    }
}
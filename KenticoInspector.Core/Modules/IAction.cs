using System;
using System.Collections.Generic;

using KenticoInspector.Core.Models;

namespace KenticoInspector.Core.Modules
{
    public interface IAction : IModule
    {
        ActionResults Execute<T>(T ActionOptions) where T : new();
    }
}
using System;
using System.Collections.Generic;

using KenticoInspector.Core.Models;

namespace KenticoInspector.Core.Services.Interfaces
{
    public interface IVersionService : IService
    {
        string GetCoreProductVersion();
    }
}
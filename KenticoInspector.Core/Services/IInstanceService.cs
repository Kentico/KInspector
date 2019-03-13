using KenticoInspector.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace KenticoInspector.Core.Services
{
    public interface IInstanceService
    {
        Instance GetInstance(InstanceConfiguration instanceConfiguration);
    }
}

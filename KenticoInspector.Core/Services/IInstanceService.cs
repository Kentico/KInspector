using KenticoInspector.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace KenticoInspector.Core.Services
{
    interface IInstanceService
    {
        Instance ConnectToInstance(InstanceConfiguration instanceConfiguration);
    }
}

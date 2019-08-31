using System;
using System.Collections.Generic;

using KenticoInspector.Core.Models;
using KenticoInspector.Core.Modules;
using KenticoInspector.Core.Services.Interfaces;

namespace KenticoInspector.Core
{
    public abstract class AbstractAction<T> : AbstractModule<T>, IAction where T : new()
    {
        public AbstractAction(IModuleMetadataService moduleMetadataService) : base(moduleMetadataService)
        {
        }

        public abstract ActionResults Execute<TOptions>(TOptions ActionOptions) where TOptions : new();
    }
}
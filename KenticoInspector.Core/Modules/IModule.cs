using System;
using System.Collections.Generic;

namespace KenticoInspector.Core.Modules
{
    public interface IModule
    {
        string Codename { get; }

        IList<Version> CompatibleVersions { get; }

        IList<Version> IncompatibleVersions { get; }

        IList<string> Tags { get; }
    }
}
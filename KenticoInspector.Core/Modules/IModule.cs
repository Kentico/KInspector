using Newtonsoft.Json;
using System;
using System.Collections.Generic;

using KenticoInspector.Core.Converters;

namespace KenticoInspector.Core.Modules
{
    public interface IModule
    {
        string Codename { get; }

        [JsonConverter(typeof(VersionListConverter))]
        IList<Version> CompatibleVersions { get; }

        [JsonConverter(typeof(VersionListConverter))]
        IList<Version> IncompatibleVersions { get; }

        IList<string> Tags { get; }
    }
}
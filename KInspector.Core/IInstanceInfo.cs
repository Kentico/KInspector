using System;
namespace Kentico.KInspector.Core
{
    public interface IInstanceInfo
    {
        InstanceConfig Config { get; }
        IDatabaseService DBService { get; }
        System.IO.DirectoryInfo Directory { get; }
        Uri Uri { get; }
        Version Version { get; }
    }
}

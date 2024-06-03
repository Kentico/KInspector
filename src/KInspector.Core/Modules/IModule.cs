namespace KInspector.Core.Modules
{
    /// <summary>
    /// A module that can be executed against the connected instance.
    /// </summary>
    public interface IModule
    {
        /// <summary>
        /// The module code name.
        /// </summary>
        string Codename { get; }

        /// <summary>
        /// A list of Kentico major versions which are supported by the module.
        /// </summary>
        IList<Version> CompatibleVersions { get; }

        /// <summary>
        /// A list of Kentico major versions which are not supported by the module.
        /// </summary>
        IList<Version> IncompatibleVersions { get; }

        /// <summary>
        /// The module tags found in <see cref="Constants.ModuleTags"/>.
        /// </summary>
        IList<string> Tags { get; }
    }
}
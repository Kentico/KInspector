namespace KInspector.Blazor.Services
{
    /// <summary>
    /// Stores data to be accessed by Blazor components and allows components to react to data modification.
    /// </summary>
    public class StateContainer
    {
        private readonly List<string> runningModules = new();

        public event Action? OnChange;

        private void NotifyStateChanged() => OnChange?.Invoke();

        /// <summary>
        /// Adds a module to the list of running modules.
        /// </summary>
        public void AddModule(string codename)
        {
            runningModules.Add(codename);
            NotifyStateChanged();
        }

        /// <summary>
        /// Removes a module from the list of running modules.
        /// </summary>
        public void RemoveModule(string codename)
        {
            runningModules.Remove(codename);
            NotifyStateChanged();
        }

        public bool Contains(string codename) => runningModules.Contains(codename);
    }
}

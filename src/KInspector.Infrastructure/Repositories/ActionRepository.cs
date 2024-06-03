using KInspector.Core.Modules;
using KInspector.Core.Repositories.Interfaces;

namespace KInspector.Infrastructure.Repositories
{
    public class ActionRepository : IActionRepository
    {
        private readonly IEnumerable<IAction> actions;

        public ActionRepository(IEnumerable<IAction> actions)
        {
            this.actions = actions;
        }

        public IAction? GetAction(string codename) =>
            actions.FirstOrDefault(x => x.Codename.Equals(codename, StringComparison.InvariantCultureIgnoreCase));

        public IEnumerable<IAction> GetActions() => actions;
    }
}
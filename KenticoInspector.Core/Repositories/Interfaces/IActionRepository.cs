using System.Collections.Generic;

using KenticoInspector.Core.Modules;

namespace KenticoInspector.Core.Repositories.Interfaces
{
    public interface IActionRepository : IRepository
    {
        IEnumerable<IAction> GetActions();

        IAction GetAction(string codename);
    }
}
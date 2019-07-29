using KenticoInspector.Core.Models;
using System;

namespace KenticoInspector.Core.Repositories.Interfaces
{
    public interface IVersionRepository : IRepository
    {
        Version GetKenticoAdministrationVersion(Instance instance);

        Version GetKenticoAdministrationVersion(string rootPath);

        Version GetKenticoDatabaseVersion(Instance instance);

        Version GetKenticoDatabaseVersion(DatabaseSettings databaseSettings);
    }
}
using KenticoInspector.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace KenticoInspector.Core.Repositories.Interfaces
{
    public interface IVersionRepository
    {
        Version GetKenticoAdministrationVersion(Instance instance);
        Version GetKenticoAdministrationVersion(string rootPath);
        Version GetKenticoDatabaseVersion(Instance instance);
        Version GetKenticoDatabaseVersion(DatabaseSettings databaseSettings);
    }
}

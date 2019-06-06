using KenticoInspector.Core.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace KenticoInspector.Core.Services.Interfaces
{
    public interface IDatabaseService : IService
    {
        void ConfigureForInstance(Instance instance);

        IEnumerable<T> ExecuteSqlFromFile<T>(string relativeFilePath, dynamic parameters = null);

        IEnumerable<IDictionary<string, object>> ExecuteSqlFromFileWithReplacements(string relativeFilePath, IDictionary<string,string> replacements, dynamic parameters = null);

        [Obsolete("This should be a last resort when it is impossible to create a DTO")]
        DataTable ExecuteSqlFromFileAsDataTable(string relativeFilePath);
        
    }
}

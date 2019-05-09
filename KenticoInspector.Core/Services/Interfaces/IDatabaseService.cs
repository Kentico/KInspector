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

        IEnumerable<T> ExecuteSqlFromFile<T>(string relativeFilePath, object parameters = null);

        DataTable ExecuteSqlFromFileAsDataTable(string relativeFilePath);
    }
}

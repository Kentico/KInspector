using System;
using System.Collections.Generic;
using System.Data;

using KenticoInspector.Core.Models;

namespace KenticoInspector.Core.Services.Interfaces
{
    public interface IDatabaseService : IService
    {
        void Configure(DatabaseSettings databaseSettings);

        IEnumerable<T> ExecuteSqlFromFile<T>(string relativeFilePath);

        IEnumerable<T> ExecuteSqlFromFile<T>(string relativeFilePath, dynamic parameters);

        IEnumerable<T> ExecuteSqlFromFile<T>(string relativeFilePath, IDictionary<string, string> literalReplacements);

        IEnumerable<T> ExecuteSqlFromFile<T>(string relativeFilePath, IDictionary<string, string> literalReplacements, dynamic parameters);

        [Obsolete("A last resort when it is impossible to create a data model or use one of the generic options.")]
        DataTable ExecuteSqlFromFileAsDataTable(string relativeFilePath);

        IEnumerable<IDictionary<string, object>> ExecuteSqlFromFileGeneric(string relativeFilePath);

        IEnumerable<IDictionary<string, object>> ExecuteSqlFromFileGeneric(string relativeFilePath, dynamic parameters);

        IEnumerable<IDictionary<string, object>> ExecuteSqlFromFileGeneric(string relativeFilePath, IDictionary<string, string> literalReplacements);

        IEnumerable<IDictionary<string, object>> ExecuteSqlFromFileGeneric(string relativeFilePath, IDictionary<string, string> literalReplacements, dynamic parameters);

        T ExecuteSqlFromFileScalar<T>(string relativeFilePath);

        T ExecuteSqlFromFileScalar<T>(string relativeFilePath, dynamic parameters);

        T ExecuteSqlFromFileScalar<T>(string relativeFilePath, IDictionary<string, string> literalReplacements);

        T ExecuteSqlFromFileScalar<T>(string relativeFilePath, IDictionary<string, string> literalReplacements, dynamic parameters);
    }
}
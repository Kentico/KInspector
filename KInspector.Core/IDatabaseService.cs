using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Kentico.KInspector.Core
{
    public interface IDatabaseService
    {
        DataSet ExecuteAndGetDataSetFromFile(string filePath);

        List<string> ExecuteAndGetPrintsFromFile(string filePath);

        T ExecuteAndGetScalar<T>(string sql) where T : IConvertible;

        DataTable ExecuteAndGetTableFromFile(string filePath, params SqlParameter[] parameters);

        DataSet ExecuteAndGetDataSetFromFile(string filePath, params SqlParameter[] parameters);

        T GetSetting<T>(string key, string siteName = "") where T : IConvertible;

        DataSet ExecuteAndGetDataSet(string sql, params SqlParameter[] parameters);
    }
}
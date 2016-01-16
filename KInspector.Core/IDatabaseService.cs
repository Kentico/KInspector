using System;
namespace Kentico.KInspector.Core
{
    public interface IDatabaseService
    {
        System.Data.DataSet ExecuteAndGetDataSetFromFile(string filePath);
        System.Collections.Generic.List<string> ExecuteAndGetPrintsFromFile(string filePath);
        T ExecuteAndGetScalar<T>(string sql) where T : IConvertible;
        System.Data.DataTable ExecuteAndGetTableFromFile(string filePath, params System.Data.SqlClient.SqlParameter[] parameters);
        T GetSetting<T>(string key, string siteName = "") where T : IConvertible;
    }
}

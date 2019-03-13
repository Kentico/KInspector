using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace KenticoInspector.Core.Services
{
    public interface IDatabaseService
    {
        DataSet ExecuteAndGetDataSet(string sql, params IDbDataParameter[] parameters);

        DataSet ExecuteAndGetDataSetFromFile(string relativeFilePath, params IDbDataParameter[] parameters);
        
        DataTable ExecuteAndGetDataTable(string sql, params IDbDataParameter[] parameters);

        DataTable ExecuteAndGetDataTableFromFile(string relativeFilePath, params IDbDataParameter[] parameters);

        T ExecuteAndGetScalar<T>(string sql) where T : IConvertible;
    }
}

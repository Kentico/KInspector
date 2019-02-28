using System;
using System.Data;

namespace Kentico.KInspector.Modules.Helpers.StrongTypedInfos
{
    public class SimpleBaseInfo
    {
        private readonly DataRow _row;

        protected T Get<T>(string columnName)
        {
            if (_row[columnName] is DBNull)
            {
                return default(T);
            }
            return (T)_row[columnName];
        }

        protected SimpleBaseInfo(DataRow row)
        {
            _row = row;
        }
    }
}
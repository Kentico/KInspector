using System;
using System.Collections.Generic;
using System.Text;

namespace KenticoInspector.Reports.ClassTableValidation
{
    public static class Scripts
    {
        public const string BaseDirectory = "ClassTableValidation/";
        public const string ClassesWithNoTable = BaseDirectory + "GetClassesWithNoTable.sql";
        public const string TablesWithNoClass = BaseDirectory + "GetTablesWithNoClass.sql";
    }
}

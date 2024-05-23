﻿namespace KInspector.Reports.ContentTreeConsistencyAnalysis.Models
{
    public class CmsClassField
    {
        public string? Caption { get; set; }

        public string? Column { get; set; }

        public string? ColumnType { get; set; }

        public string? DefaultValue { get; set; }

        public bool IsIdColumn { get; set; }
    }
}
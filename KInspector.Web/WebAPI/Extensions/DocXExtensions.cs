using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Novacode;

namespace Kentico.KInspector.Extensions
{
    public static class DocXExtensions
    {
        public static void InsertDataTable(this DocX document, DataTable table)
        {
            if (table != null && table.Columns.Count != 0 && table.Rows.Count != 0)
            {
                var documentTable = document.InsertTable(table.Rows.Count + 1, table.Columns.Count);

                // Insert headers
                for (int headerIndex = 0; headerIndex < table.Columns.Count; headerIndex++)
                {
                    documentTable.Rows[0].Cells[headerIndex].Paragraphs[0].InsertText(Convert.ToString(table.Columns[headerIndex]),
                        false, new Formatting { Bold = true });
                }

                // Insert row data
                for (int row = 0; row < table.Rows.Count; row++)
                {
                    for (int column = 0; column < table.Columns.Count; column++)
                    {
                        documentTable.Rows[row + 1].Cells[column].Paragraphs[0].InsertText(Convert.ToString(table.Rows[row][column]));
                    }
                }
            }
        }
    }
}

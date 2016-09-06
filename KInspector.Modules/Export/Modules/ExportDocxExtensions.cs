using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using NPOI.XWPF.UserModel;

namespace Kentico.KInspector.Modules.Export.Modules
{
    /// <summary>
    /// Extensions for NPOI docx manipulations. Used primarily in <see cref="ExportDocx"/>.
    /// </summary>
    public static class ExportDocxExtensions
    {
        /// <summary>
        /// Create new text run in paragraph by merging provided strings.
        /// </summary>
        /// <param name="paragraph">Paragraph to write into.</param>
        /// <param name="data">Data to concatenate and write into the run.</param>
        /// <returns>Newly created text run inside the paragraph.</returns>
        public static XWPFRun CreateRun(this XWPFParagraph paragraph, params string[] data)
        {
            if (paragraph == null)
            {
                throw new ArgumentNullException(nameof(paragraph));
            }

            var run = paragraph.CreateRun();
            run.SetText(string.Join(" ", data));

            return run;
        }

        /// <summary>
        /// Create new text run in paragraph by merging provided strings.
        /// </summary>
        /// <param name="document">Document to create the paragraph in.</param>
        /// <param name="data">Data to create as runs.</param>
        /// <returns>Newly created paragraph.</returns>
        public static XWPFParagraph CreateParagraph(this XWPFDocument document, params string[] data)
        {
            if (document == null)
            {
                throw new ArgumentNullException(nameof(document));
            }

            var par = document.CreateParagraph();

            foreach (string val in data)
            {
                par.CreateRun(val);
            }

            return par;
        }

        /// <summary>
        /// Fill table row with data
        /// </summary>
        /// <param name="row">Row to write into.</param>
        /// <param name="cellData">Cell data to write into the row.</param>
        /// <returns>The row which has been written into.</returns>
        public static XWPFTableRow FillRow(this XWPFTableRow row, params string[] cellData)
        {
            return row.FillRow(cellData as IEnumerable<string>);
        }

        /// <summary>
        /// Fill table row with data
        /// </summary>
        /// <param name="row">Row to write into.</param>
        /// <param name="cellData">Cell data to write into the row.</param>
        /// <returns>The row which has been written into.</returns>
        public static XWPFTableRow FillRow(this XWPFTableRow row, params object[] cellData)
        {
            return row.FillRow(cellData as IEnumerable<object>);
        }

        /// <summary>
        /// Fill table row with data
        /// </summary>
        /// <param name="row">Row to write into.</param>
        /// <param name="cellData">Cell data to write into the row.</param>
        /// <returns>The row which the data has been written into.</returns>
        public static XWPFTableRow FillRow<T>(this XWPFTableRow row, IEnumerable<T> cellData)
        {
            if (row == null)
            {
                throw new ArgumentNullException(nameof(row));
            }

            int ii = 0;
            foreach (var val in cellData)
            {
                var cell = row.GetCell(ii) ?? row.AddNewTableCell();
                cell.SetText(Convert.ToString(val));

                ii++;
            }

            return row;
        }

        /// <summary>
        /// Fill table rows with data.
        /// </summary>
        /// <param name="table">Table to fill.</param>
        /// <param name="data">Data to write.</param>
        /// <returns>The table which the data has been written into.</returns>
        public static XWPFTable FillRows(this XWPFTable table, DataTable data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            // Insert table headers
            table.GetRow(0).FillRow(data.Columns.Cast<DataColumn>().Select(column => column.ColumnName));

            // Insert row data
            int ii = 1;
            foreach (DataRow dataRow in data.Rows)
            {
                var row = table.GetRow(ii) ?? table.CreateRow();
                row.FillRow(dataRow.ItemArray);

                ii++;
            }

            return table;
        }

        /// <summary>
        /// Fill table rows with data.
        /// </summary>
        /// <param name="table">Table to fill.</param>
        /// <param name="data">Data to write.</param>
        /// <returns>The table which the data has been written into.</returns>
        public static XWPFTable FillTable<T>(this XWPFTable table, IEnumerable<T> data)
        {
            if (table == null)
            {
                throw new ArgumentNullException(nameof(table));
            }

            int ii = 0;
            foreach (var val in data)
            {
                // Get or create table row
                var row = table.GetRow(ii) ?? table.CreateRow();

                row.FillRow(Convert.ToString(val));

                ii++;
            }

            return table;
        }
    }
}
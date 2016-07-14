using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using Novacode;
using NPOI.SS.UserModel;
using NPOI.WP.UserModel;
using NPOI.XWPF.UserModel;

namespace Kentico.KInspector.Modules
{
    public static class ExportExtensions
    {
        #region Xlsx extensions

        #region Create next empty

        /// <summary>
        /// Create new cell at the end of row.
        /// </summary>
        /// <param name="row">Row where to add the cell.</param>
        /// <returns>Newly created cell.</returns>
        public static NPOI.SS.UserModel.ICell CreateCell(this IRow row)
        {
            if (row == null)
            {
                throw new ArgumentNullException(nameof(row));
            }

            // LastCellNum returns -1 if there are no cells
            short newCellIndex = (row.LastCellNum != -1) ? row.LastCellNum : (short)0;

            // TODO: Find and enforce max width limit

            return row.CreateCell(newCellIndex);
        }

        /// <summary>
        /// Create new row at the end of sheet.
        /// </summary>
        /// <param name="sheet">Sheet where to add the sheet.</param>
        /// <returns>Newly created row.</returns>
        public static IRow CreateRow(this ISheet sheet)
        {
            if (sheet == null)
            {
                throw new ArgumentNullException(nameof(sheet));
            }
            
            int newRowIndex = (sheet.PhysicalNumberOfRows > 0) ? sheet.LastRowNum + 1 : 0;

            return sheet.CreateRow(newRowIndex);
        }

        #endregion

        #region Create single row with data

        /// <summary>
        /// Create row with data at the end of the sheet.
        /// </summary>
        /// <param name="sheet">Sheet where to add the row.</param>
        /// <param name="data">Row cell data.</param>
        public static IRow CreateRow(this ISheet sheet, params string[] data)
        {
            return sheet.CreateRow(data as IEnumerable<string>);
        }

        /// <summary>
        /// Create row with data at the end of the sheet.
        /// </summary>
        /// <param name="sheet">Sheet where to add the row.</param>
        /// <param name="data">Row cell data.</param>
        public static IRow CreateRow(this ISheet sheet, params object[] data)
        {
            return sheet.CreateRow(data as IEnumerable<object>);
        }

        /// <summary>
        /// Create row with data at the end of the sheet.
        /// </summary>
        /// <typeparam name="T">Type of data to add.</typeparam>
        /// <param name="sheet">Sheet where to add the row.</param>
        /// <param name="data">Data to insert.</param>
        public static IRow CreateRow<T>(this ISheet sheet, IEnumerable<T> data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            var row = sheet.CreateRow();

            foreach (var val in data)
            {
                row.CreateCell().SetCellValue(Convert.ToString(val));
            }

            return row;
        }

        #endregion

        #region Create multiple rows with data

        /// <summary>
        /// Create multiple rows with data at the end of the sheet.
        /// </summary>
        /// <typeparam name="T">Type of data to add.</typeparam>
        /// <param name="sheet">Sheet where to add the row.</param>
        /// <param name="data">Cells data.</param>
        /// <returns>Last inserted row.</returns>
        public static IRow CreateRows<T>(this ISheet sheet, IEnumerable<T> data)
        {
            IRow lastRow = null;

            foreach (var val in data)
            {
                lastRow = sheet.CreateRow(Convert.ToString(val));
            }

            return lastRow;
        }

        /// <summary>
        /// Create multiple rows with data at the end of the sheet.
        /// </summary>
        /// <param name="sheet">Sheet where to add the row.</param>
        /// <param name="data">Cells data.</param>
        /// <returns>Last inserted row.</returns>
        public static IRow CreateRows(this ISheet sheet, DataTable data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            // Insert table headers
            sheet.CreateRow(data.Columns.Cast<DataColumn>().Select(column => column.ColumnName));
            // TODO: Set header style

            IRow lastRow = null;

            // Insert row data
            foreach (DataRow row in data.Rows)
            {
                lastRow = sheet.CreateRow(row.ItemArray);
            }

            return lastRow;
        }

        #endregion

        #endregion

        #region docx

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

        public static XWPFTableRow FillRow(this XWPFTableRow row, params string[] data)
        {
            return row.FillRow(data as IEnumerable<string>);
        }

        public static XWPFTableRow FillRow(this XWPFTableRow row, params object[] data)
        {
            return row.FillRow(data as IEnumerable<object>);
        }

        public static XWPFTableRow FillRow<T>(this XWPFTableRow row, IEnumerable<T> data)
        {
            if (row == null)
            {
                throw new ArgumentNullException(nameof(row));
            }

            int ii = 0;
            foreach (var val in data)
            {
                var cell = row.GetCell(ii) ?? row.AddNewTableCell();
                cell.SetText(Convert.ToString(val));

                ii++;
            }

            return row;
        }

        public static XWPFTable FillRows(this XWPFTable table, DataTable data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            // Insert table headers
            table.GetRow(0).FillRow(data.Columns.Cast<DataColumn>().Select(column => column.ColumnName));

            // TODO: Set header style
            
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

        #region obsolete

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

        #endregion

        #endregion
    }
}

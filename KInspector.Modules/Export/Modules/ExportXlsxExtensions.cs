using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using NPOI.SS.UserModel;

namespace Kentico.KInspector.Modules.Export.Modules
{
    /// <summary>
    /// Extensions for NPOI xlsx manipulations. Used primarily in <see cref="ExportXlsx"/>.
    /// </summary>
    public static class ExportXlsxExtensions
    {
        #region Create next empty

        /// <summary>
        /// Create new cell at the end of row.
        /// </summary>
        /// <param name="row">Row where to add the cell.</param>
        /// <returns>Newly created cell.</returns>
        public static ICell CreateCell(this IRow row)
        {
            if (row == null)
            {
                throw new ArgumentNullException(nameof(row));
            }

            // LastCellNum returns -1 if there are no cells
            short newCellIndex = (row.LastCellNum != -1) ? row.LastCellNum : (short)0;

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

            IRow lastRow = null;

            // Insert row data
            foreach (DataRow row in data.Rows)
            {
                lastRow = sheet.CreateRow(row.ItemArray);
            }

            return lastRow;
        }

        #endregion
    }
}
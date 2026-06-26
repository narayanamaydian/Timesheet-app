using System.Data;

namespace Timesheet_app.Helper
{
    public class DataTableHelper
    {
        /// <summary>
        /// Converts a DataTable to a List of Dictionaries for JSON-friendly serialization.
        /// Each dictionary represents a row with column names as keys and cell values as values.
        /// </summary>
        public static List<Dictionary<string, object>> ToJsonFriendly(DataTable table)
        {
            var list = new List<Dictionary<string, object>>();

            foreach (DataRow row in table.Rows)
            {
                var dict = new Dictionary<string, object>();

                foreach (DataColumn col in table.Columns)
                {
                    dict[col.ColumnName] = row[col] == DBNull.Value ? null : row[col];
                }

                list.Add(dict);
            }

            return list;
        }
    }
}

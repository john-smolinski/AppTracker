using ApplicationTracker.Data;
using ApplicationTracker.Data.Entities;
using ClosedXML.Excel;
using System.Text;


namespace ApplicationTracker.ImportCli.Helpers
{
    /// <summary>
    /// simple helper class for code reuse in reporting and importing
    /// </summary>
    public static class DataHelper
    {
        public static Dictionary<Type, List<string>> GetMappings()
        {
            return new Dictionary<Type, List<string>>
            {
                { typeof(Source), new List<string>{ "B" } },
                { typeof(Organization), new List<string>{ "C" } },
                { typeof(JobTitle), new List<string>{ "D" } },
                { typeof(WorkEnvironment), new List<string>{ "E" } }
            };
        }

        public static HashSet<string> ExtractColumnValues(IXLWorksheet workSheet, List<string> columns)
        {
            var values = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var column in columns)
            {
                var col = workSheet.Column(column);

                // ignore the header row
                foreach (var cell in col.CellsUsed().Skip(1))
                {
                    string value = cell.GetValue<string>().Trim();
                    if (!string.IsNullOrEmpty(value))
                    {
                        values.Add(value);
                    }
                }
            }
            return values;
        }

        public static HashSet<KeyValuePair<string?, string>> ExtractLocations(IXLWorksheet worksheet)
        {
            var values = new HashSet<KeyValuePair<string?, string>>();

            var cityCol = worksheet.Column("F");
            var stateCol = worksheet.Column("G");

            // find last row with a value
            int lastCity = cityCol.LastCellUsed()?.Address.RowNumber ?? 0;
            int lastState = stateCol.LastCellUsed()?.Address?.RowNumber ?? 0;

            int lastRow = Math.Max(lastCity, lastState);

            // skip header row
            for (int row = 2; row <= lastRow; row++)
            {
                var city = cityCol.Cell(row).GetValue<string>().Trim();
                var state = stateCol.Cell(row).GetValue<string>().Trim();

                // skip where both are empty
                if (string.IsNullOrWhiteSpace(city) && string.IsNullOrWhiteSpace(state))
                    continue;

                var location = new KeyValuePair<string?, string>(
                    string.IsNullOrEmpty(city) ? null : city, state);

                values.Add(location);
            }
            return values;
        }
    }
}

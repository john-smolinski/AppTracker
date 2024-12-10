using ApplicationTracker.Data;
using ApplicationTracker.Data.Entities;
using ClosedXML.Excel;
using System.Text;


namespace ApplicationTracker.ImportCli
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

            // TODO: add multi column values
            // single-column entities (common case)
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
    }
}

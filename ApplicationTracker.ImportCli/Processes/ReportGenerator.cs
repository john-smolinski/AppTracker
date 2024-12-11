using ApplicationTracker.Data;
using ApplicationTracker.Data.Entities;
using ApplicationTracker.ImportCli.Helpers;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace ApplicationTracker.ImportCli.Processes
{
    public class ReportGenerator
    {
        private readonly TrackerDbContext _context;
        private Dictionary<Type, List<string>> _mappings;

        public ReportGenerator(TrackerDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mappings = DataHelper.GetMappings();
        }

        public string GenerateReport<T>(IXLWorksheet workSheet)
            where T : BaseEntity
        {
            ArgumentNullException.ThrowIfNull(workSheet);

            if (!_mappings.TryGetValue(typeof(T), out var columns) || columns == null || columns.Count == 0)
                throw new InvalidOperationException($"No column mappings found for type {typeof(T).Name}.");

            HashSet<string> entities = DataHelper.ExtractColumnValues(workSheet, columns);

            // empty column 
            if (entities.Count == 0)
            {
                return $"No data found in worksheet for {typeof(T).Name}.";
            }

            StringBuilder result = new();

            foreach (var entity in entities)
            {
                var report = ReportOnEntity<T>(entity);
                if (report != string.Empty)
                {
                    result.AppendLine(report);
                }
            }

            return FormatResult<T>(result);
        }

        public string GenerateLocationReport(IXLWorksheet workSheet)
        {
            ArgumentNullException.ThrowIfNull(workSheet);

            var cityColumn = workSheet.Column("F");
            var stateColumn = workSheet.Column("G");

            // find last row with a value
            int lastCity = cityColumn.LastCellUsed()?.Address.RowNumber ?? 0;
            int lastState = stateColumn.LastCellUsed()?.Address?.RowNumber ?? 0;

            int lastRow = Math.Max(lastCity, lastState);

            StringBuilder result = new();

            // skip header row
            for (int row = 2; row <= lastRow; row++)
            {
                var city = cityColumn.Cell(row).GetValue<string>().Trim();
                var state = stateColumn.Cell(row).GetValue<string>().Trim();

                // skip where both are empty
                if (string.IsNullOrWhiteSpace(city) && string.IsNullOrWhiteSpace(state))
                    continue;

                var report = ReportOnLocation(city, state);
                if (report != string.Empty)
                {
                    result.AppendLine(report);
                }
            }
            return result.ToString();
        }

        /// <summary>
        /// for reporting on simple entities 
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="value">value to test</param>
        /// <returns></returns>
        private string ReportOnEntity<T>(string value)
            where T : BaseEntity
        {
            // retrieve the DbSet dynamically
            var dbSet = _context.Set<T>();

            // check if the entity exists in the database
            var entity = dbSet.FirstOrDefault(e => e.Name.Equals(value));
            return entity == null ? $"> {value}" : string.Empty;
        }

        /// <summary>
        /// for reporting on Locations only
        /// </summary>
        /// <param name="city"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        private string ReportOnLocation(string city, string state)
        {
            var location = _context.Locations.FirstOrDefault(x =>
                (string.IsNullOrEmpty(city) ? x.City == null : x.City!.Equals(city))
                && x.State!.Equals(state));

            if (location == null)
            {
                return $">{(string.IsNullOrEmpty(city) ? "" : city)} - {state}";

            }
            return string.Empty;
        }

        private static string FormatResult<T>(StringBuilder result)
            where T : BaseEntity
        {
            if (result.Length > 0)
            {
                return $"Use the -e (execute) option to add the following {typeof(T).Name} objects:\n{result}";
            }
            return $"> All {typeof(T).Name} objects are up to date.";
        }
    }
}

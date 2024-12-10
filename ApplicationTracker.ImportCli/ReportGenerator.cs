using ApplicationTracker.Data;
using ApplicationTracker.Data.Entities;
using ClosedXML.Excel;
using System.Text;

namespace ApplicationTracker.ImportCli
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
            if(entities.Count == 0)
            {
                return $"No data found in worksheet for {typeof(T).Name}.";
            }

            StringBuilder result = new();

            foreach(var entity in entities)
            {
                var report = ReportOnEntity<T>(entity);
                if(report != string.Empty)
                {
                    result.AppendLine(report);
                }
            }

            return FormatResult<T>(result);
        }


        private string ReportOnEntity<T>(string value)
            where T : BaseEntity
        {
            // retrieve the DbSet dynamically
            var dbSet = _context.Set<T>();

            // check if the entity exists in the database
            var entity = dbSet.FirstOrDefault(e => e.Name.Equals(value));
            return entity == null ? $"> {value}" : string.Empty;
        }

        private string FormatResult<T>(StringBuilder result)
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

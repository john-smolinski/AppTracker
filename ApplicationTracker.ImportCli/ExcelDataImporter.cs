using ApplicationTracker.Data;
using ApplicationTracker.Data.Entities;
using ClosedXML.Excel;
using System.Text;

namespace ApplicationTracker.ImportCli
{
    public class ExcelDataImporter
    {
        private readonly TrackerDbContext _context;
        private readonly Dictionary<Type, List<string>> _mappings;

        public ExcelDataImporter(TrackerDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mappings = DataHelper.GetMappings();
        }

        public string ImportEntities<T>(IXLWorksheet workSheet)
            where T : BaseEntity, new()
        {
            ArgumentNullException.ThrowIfNull(workSheet);

            if (!_mappings.TryGetValue(typeof(T), out var columns) || columns == null || columns.Count == 0)
            {
                throw new InvalidOperationException($"No column mappings found for type {typeof(T).Name}.");
            }

            HashSet<string> entities = DataHelper.ExtractColumnValues(workSheet, columns);

            // empty column
            if (entities.Count == 0)
            {
                return $"No data found in worksheet for {typeof(T).Name}.";
            }

            StringBuilder result = new();

            foreach (var entity in entities)
            {
                var report = InsertEntity<T>(entity);
                result.AppendLine(report);
            }
            return result.ToString();
        }

        private string InsertEntity<T>(string value)
            where T : BaseEntity, new()
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Entity value cannot be null or empty.", nameof(value));
            }

            var dbSet = _context.Set<T>();

            var entity = dbSet.FirstOrDefault(e => e.Name.Equals(value));

            if (entity == null)
            {
                try
                {
                    var newEntity = new T
                    {
                        Name = value
                    };
                    dbSet.Add(newEntity);
                    _context.SaveChanges();

                    return $"> {typeof(T).Name} '{value}' inserted ";
                }
                catch (Exception ex)
                {
                    return $"! Error inserting {typeof(T).Name} '{value}': {ex.Message}";
                }
            }
            else
            {
                return $"! {typeof(T).Name} {value} already exists";
            }
        }
    }
}

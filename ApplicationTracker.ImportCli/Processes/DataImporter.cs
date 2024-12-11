using ApplicationTracker.Data;
using ApplicationTracker.Data.Entities;
using ApplicationTracker.ImportCli.Helpers;
using ClosedXML.Excel;
using System.Text;

namespace ApplicationTracker.ImportCli.Processes
{
    public class DataImporter(TrackerDbContext context)
    {
        private readonly TrackerDbContext _context = context ?? throw new ArgumentNullException(nameof(context));
        private readonly Dictionary<Type, List<string>> _mappings = DataHelper.GetMappings();

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

        public string ImportLocations(IXLWorksheet workSheet)
        {
            ArgumentNullException.ThrowIfNull(workSheet);

            HashSet<KeyValuePair<string?, string>> locations = DataHelper.ExtractLocations(workSheet);

            if (locations.Count == 0)
            {
                return "No Location data found in worksheet";
            }

            var existingLocations = new HashSet<KeyValuePair<string?, string>>(
                _context.Locations
                    .Where(s => s.State != null)
                    .Select(x =>
                        new KeyValuePair<string?, string>(x.City, x.State!)),
                        new LocationEqualityComparer()
                );

            StringBuilder result = new();

            foreach (var pair in locations)
            {
                if (!existingLocations.Contains(new KeyValuePair<string?, string>(pair.Key, pair.Value)))
                {
                    // all entities inherit from BaseEntity with name being required
                    var location = new Location
                    {
                        Name = $"{(string.IsNullOrWhiteSpace(pair.Key) ? pair.Value : pair.Key )}|{pair.Value}",
                        City = pair.Key,
                        State = pair.Value
                    };
                    _context.Locations.Add(location);
                    result.AppendLine($"> {pair.Key ?? "null"} - {pair.Value} inserted");
                }
            }

            if (result.Length > 0)
            {
                _context.SaveChanges();
            }
            else
            {
                result.AppendLine("Locations already up to date.");
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

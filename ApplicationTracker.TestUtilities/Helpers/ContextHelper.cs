using ApplicationTracker.Data;
using ApplicationTracker.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace ApplicationTracker.TestUtilities.Helpers
{
    [ExcludeFromCodeCoverage]
    public static class ContextHelper
    {
        public static TrackerDbContext GetInMemoryContext<T>(int rows = 0)
            where T : BaseEntity, new()
        {
            var options = new DbContextOptionsBuilder<TrackerDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new TrackerDbContext(options);

            if (rows > 0)
            {
                if(typeof(T) != typeof(Location))
                {
                    AddTestEntities<T>(context, rows);
                }
                else
                {
                    AddTestLocations(context, rows);
                }
            }
            return context;
        }

        public static void AddTestEntities<T>(TrackerDbContext context, int count)
            where T : BaseEntity, new()
        {
            var entities = GenerateTestEntities<T>(count);
            context.Set<T>().AddRange(entities);
            context.SaveChanges();
        }

        public static IEnumerable<T> GenerateTestEntities<T>(int count)
            where T : BaseEntity, new()
        {
            return Enumerable.Range(1, count).Select(i => new T
            {
                Id = i,
                Name = $"Test {typeof(T).Name} {i}"
            });
        }

        public static void AddTestLocations(TrackerDbContext context, int count)
        {
            var entities = Enumerable.Range(1, count).Select(i => new Location
            {
                Id = i,
                Name = $"Test {typeof(Location).Name} {i}",
                State= $"S{i}"
            });
            context.AddRange(entities);
            context.SaveChanges();
        }
    }
}

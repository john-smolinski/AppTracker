using ApplicationTracker.Data;
using ApplicationTracker.Data.Entities;
using ApplicationTracker.Data.Enum;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace ApplicationTracker.TestUtilities.Helpers
{
    [ExcludeFromCodeCoverage]
    public static class ContextHelper
    {
        public static TrackerDbContext GetInMemoryContext<T>(int rows = 0)
            where T : class, new()
        {
            var options = new DbContextOptionsBuilder<TrackerDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new TrackerDbContext(options);

            if (rows > 0)
            {
                if (typeof(T) == typeof(Application))
                {
                    AddTestApplications(context, rows);
                }
                else if (typeof(BaseEntity).IsAssignableFrom(typeof(T)))
                {
                    AddBaseEntityTestEntities(context, typeof(T), rows);
                }

            }

            return context;
        }

        public static void AddTestApplications(TrackerDbContext context, int count)
        {
            // Add related entities for foreign keys
            AddTestEntities<Source>(context, 5);
            AddTestEntities<Organization>(context, 5);
            AddTestEntities<JobTitle>(context, 5);
            AddTestEntities<WorkEnvironment>(context, 5);

            // Add Applications with foreign keys set
            var applications = Enumerable.Range(1, count).Select(i => new Application
            {
                Id = i,
                ApplicationDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-i)),
                SourceId = i, 
                OrganizationId = i,
                JobTitleId = i,
                WorkEnvironmentId = i,
                Source = context.Sources.Find(i)!, 
                Organization = context.Organizations.Find(i)!,
                JobTitle = context.JobTitles.Find(i)!,
                WorkEnvironment = context.WorkEnvironments.Find(i)!
            });

            context.Applications.AddRange(applications);
            context.SaveChanges();
        }

        public static void AddRejectionEvent(TrackerDbContext context, int applicationId)
        {
            var application = context.Applications.Find(applicationId) ?? 
                throw new InvalidOperationException("Application not found");
            
            var appEvent = new AppEvent
            {
                ApplicationId = applicationId,
                EventDate = DateTime.Now,
                EventType = EventType.Rejection,
                Description = "Test Rejection"
            };
            
            context.AppEvents.Add(appEvent);
            context.SaveChanges();
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

        public static void AddBaseEntityTestEntities(TrackerDbContext context, Type entityType, int count)
        {
            var method = typeof(ContextHelper)
                .GetMethod(nameof(AddTestEntities))!
                .MakeGenericMethod(entityType);

            method.Invoke(null, new object[] { context, count });
        }
    }
}

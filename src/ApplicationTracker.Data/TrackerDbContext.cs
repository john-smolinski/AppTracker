using ApplicationTracker.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace ApplicationTracker.Data
{
    [ExcludeFromCodeCoverage]
    public class TrackerDbContext : DbContext
    {
        public DbSet<Application> Applications { get; set; }
        public DbSet<WorkEnvironment> WorkEnvironments { get; set; }
        public DbSet<JobTitle> JobTitles { get; set; }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<Source> Sources { get; set; }

        public TrackerDbContext(DbContextOptions<TrackerDbContext> options)
            : base(options)
        { }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            // setup some required base data
            modelBuilder.Entity<WorkEnvironment>().HasData(
                new WorkEnvironment { Id = 1, Name = "Unknown" },
                new WorkEnvironment { Id = 2, Name = "On-Site" },
                new WorkEnvironment { Id = 3, Name = "Hybrid" },
                new WorkEnvironment { Id = 4, Name = "Remote" });

            modelBuilder.Entity<Source>().HasData(
                new Source { Id = 1, Name = "Direct" },
                new Source { Id = 2, Name = "LinkedIn" },
                new Source { Id = 3, Name = "ZipRecruiter" },
                new Source { Id = 4, Name = "Monster" },
                new Source { Id = 5, Name = "Welcome to the Jungle" });
        }

    }
}

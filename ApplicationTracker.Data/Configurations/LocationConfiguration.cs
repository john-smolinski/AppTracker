using ApplicationTracker.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApplicationTracker.Data.Configurations
{
    public class LocationConfiguration : IEntityTypeConfiguration<Location>
    {
        public void Configure(EntityTypeBuilder<Location> builder)
        {
            // Primary Key
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                   .ValueGeneratedOnAdd();

            // Relationships
            builder.HasMany(x => x.Applications)
                   .WithOne(a => a.Location) // Reference the navigation property in Application
                   .HasForeignKey(a => a.LocationId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ApplicationTracker.Data.Entities;

namespace ApplicationTracker.Data.Configurations
{
    public class LocationConfiguration : IEntityTypeConfiguration<Location>
    {
        public void Configure(EntityTypeBuilder<Location> builder) 
        { 
            builder.HasKey(x => x.Id);

            builder.HasMany(x => x.Applications)
                .WithOne()
                .HasForeignKey(x => x.Id)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

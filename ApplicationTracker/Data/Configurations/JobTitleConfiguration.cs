using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ApplicationTracker.Data.Entities;

namespace ApplicationTracker.Data.Configurations
{
    public class JobTitleConfiguration : IEntityTypeConfiguration<JobTitle>
    {
        public void Configure(EntityTypeBuilder<JobTitle> builder) 
        {
            builder.HasKey(x => x.Id);

            builder.HasIndex(x => x.Name)
                .IsUnique();

            builder.HasMany(x => x.Applications)
                .WithOne()
                .HasForeignKey(x => x.Id)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

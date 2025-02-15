using ApplicationTracker.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationTracker.Data.Configurations
{
    public class AppEventConfiguration : IEntityTypeConfiguration<AppEvent>
    {
        public void Configure(EntityTypeBuilder<AppEvent> builder)
        {
            builder.HasKey(x => x.Id);
            
            builder.HasIndex(x => x.ApplicationId);

            builder.Property(x => x.Id)
                .ValueGeneratedOnAdd();

            builder.Property(x => x.EventDate)
                .IsRequired();

            builder.Property(x => x.ContactMethod)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(x => x.EventType)
                .IsRequired()
                .HasConversion<string>();

            builder.HasOne(x => x.Application)
                .WithMany(a => a.AppEvents)
                .HasForeignKey(x => x.ApplicationId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}

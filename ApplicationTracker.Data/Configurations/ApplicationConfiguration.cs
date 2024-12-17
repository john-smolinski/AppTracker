﻿using ApplicationTracker.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApplicationTracker.Data.Configurations
{
    public class ApplicationConfiguration : IEntityTypeConfiguration<Application>
    {
        public void Configure(EntityTypeBuilder<Application> builder)
        {
            // primary key
            builder.HasKey(a => a.Id);

            builder.Property(a => a.Id)
                   .ValueGeneratedOnAdd()
                   .UseIdentityColumn();

            // relationships
            builder.HasOne(a => a.WorkEnvironment)
                   .WithMany(e => e.Applications)
                   .HasForeignKey(a => a.WorkEnvironmentId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.JobTitle)
                   .WithMany(j => j.Applications)
                   .HasForeignKey(a => a.JobTitleId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.Location)
                   .WithMany(l => l.Applications)
                   .HasForeignKey(a => a.LocationId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.Organization)
                   .WithMany(o => o.Applications)
                   .HasForeignKey(a => a.OrganizationId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.Source)
                   .WithMany(s => s.Applications)
                   .HasForeignKey(a => a.SourceId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Property(a => a.ApplicationDate)
                   .IsRequired();
        }
    }
}


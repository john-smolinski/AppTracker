﻿using ApplicationTracker.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApplicationTracker.Data.Configurations
{
    public class OrganizationConfiguration : IEntityTypeConfiguration<Organization>
    {
        public void Configure(EntityTypeBuilder<Organization> builder)
        {
            // Primary Key
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                    .ValueGeneratedOnAdd();

            // Index for unique names
            builder.HasIndex(x => x.Name)
                   .IsUnique();

            // Relationships
            builder.HasMany(x => x.Applications)
                   .WithOne(a => a.Organization) // Reference the navigation property in Application
                   .HasForeignKey(a => a.OrganizationId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

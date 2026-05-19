using ElectroPi.TaskManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectroPi.TaskManager.Infrastructure.Persistence.Configurations
{
    public sealed class ProjectConfiguration : IEntityTypeConfiguration<Project>
    {
        public void Configure(EntityTypeBuilder<Project> builder)
        {
            builder.ToTable("Projects");

            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id)
                .ValueGeneratedNever();

            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(p => p.Description)
                .HasMaxLength(1000);

            builder.Property(p => p.CreatedAt)
                .IsRequired()
                .HasColumnType("datetime2");

            builder.Property(p => p.UpdatedAt)
                .HasColumnType("datetime2");

            builder.Property(p => p.OwnerId)
                .IsRequired();

            builder.HasIndex(p => new { p.OwnerId, p.Name })
                .IsUnique()
                .HasDatabaseName("IX_Projects_OwnerId_Name");

            builder.HasIndex(p => p.OwnerId)
                .HasDatabaseName("IX_Projects_OwnerId");


            builder.HasMany(p => p.Tasks)
                .WithOne(t => t.Project)
                .HasForeignKey(t => t.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Navigation(p => p.Tasks)
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasField("_tasks");
        }
    }
}
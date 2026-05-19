using ElectroPi.TaskManager.Domain.Entities;
using ElectroPi.TaskManager.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectroPi.TaskManager.Infrastructure.Persistence.Configurations
{
    public sealed class ProjectTaskConfiguration : IEntityTypeConfiguration<ProjectTask>
    {
        public void Configure(EntityTypeBuilder<ProjectTask> builder)
        {
            builder.ToTable("ProjectTasks");

            builder.HasKey(t => t.Id);
            builder.Property(t => t.Id)
                .ValueGeneratedNever();

            builder.Property(t => t.Title)
                .IsRequired()
                .HasMaxLength(300);

            builder.Property(t => t.Description)
                .HasMaxLength(2000);

            builder.Property(t => t.Status)
        .IsRequired()
        .HasConversion<string>()
        .HasMaxLength(50);

            builder.Property(t => t.Priority)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(t => t.DueDate)
                .HasColumnType("datetime2");

            builder.Property(t => t.CreatedAt)
                .IsRequired()
                .HasColumnType("datetime2");

            builder.Property(t => t.UpdatedAt)
                .HasColumnType("datetime2");

            builder.Property(t => t.ProjectId)
                .IsRequired();

            builder.HasIndex(t => t.ProjectId)
                .HasDatabaseName("IX_ProjectTasks_ProjectId");

            builder.HasIndex(t => new { t.ProjectId, t.Status })
                .HasDatabaseName("IX_ProjectTasks_ProjectId_Status");

            builder.HasIndex(t => t.DueDate)
                .HasDatabaseName("IX_ProjectTasks_DueDate");
        }
    }
}
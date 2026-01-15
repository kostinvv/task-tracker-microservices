using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskTracker.Services.Tasks.ApplicationCore.Models;

namespace TaskTracker.Services.Tasks.Infrastructure.Data;

public class TaskItemConfiguration : IEntityTypeConfiguration<TaskItem>
{
    public void Configure(EntityTypeBuilder<TaskItem> builder)
    {
        builder.HasKey(taskItem => taskItem.Id);
        
        builder
            .Property(taskItem => taskItem.Title)
            .IsRequired()
            .HasMaxLength(TaskItem.MaxTitleLength);
        
        builder
            .Property(taskItem => taskItem.Description)
            .HasMaxLength(TaskItem.MaxDescriptionLength);
    }
}
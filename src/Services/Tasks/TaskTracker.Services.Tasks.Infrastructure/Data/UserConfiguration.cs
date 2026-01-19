using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskTracker.Services.Shared.Models;

namespace TaskTracker.Services.Tasks.Infrastructure.Data;

public class UserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder
            .HasMany(user => user.TaskItems)
            .WithOne(taskItem => taskItem.User)
            .HasForeignKey(taskItem => taskItem.UserId)
            .HasPrincipalKey(user => user.Id);
    }
}
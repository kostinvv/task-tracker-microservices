using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskTracker.Services.Shared.Data.Entities;

namespace TaskTracker.Services.Shared.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.ToTable(name: "Users", schema: "identity");
        
        builder
            .HasMany(user => user.TaskItems)
            .WithOne(taskItem => taskItem.User)
            .HasForeignKey(taskItem => taskItem.UserId)
            .HasPrincipalKey(user => user.Id);
    }
}
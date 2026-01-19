using TaskTracker.Services.Shared.Models;

namespace TaskTracker.Services.Scheduler.Data;

public class SchedulerDbContext(DbContextOptions<SchedulerDbContext> options) : DbContext(options)
{
    public DbSet<TaskItem> Tasks { get; set; }
    
    public DbSet<ApplicationUser> Users { get; set; }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        builder.Entity<ApplicationUser>()
            .HasMany(user => user.TaskItems)
            .WithOne(taskItem => taskItem.User)
            .HasForeignKey(taskItem => taskItem.UserId)
            .HasPrincipalKey(user => user.Id);
        
        builder.Entity<ApplicationUser>().ToTable(
            name: "Users", 
            schema: "identity");
    }
}
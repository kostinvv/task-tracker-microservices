using Microsoft.EntityFrameworkCore;
using TaskTracker.Services.Tasks.ApplicationCore.Models;

namespace TaskTracker.Services.Tasks.ApplicationCore.Abstractions.Context;

public interface IApplicationDbContext
{
    DbSet<ApplicationUser> Users { get; set; }
    DbSet<TaskItem> Tasks { get; set; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
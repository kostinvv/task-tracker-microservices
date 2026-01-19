using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using TaskTracker.Services.Shared.Data.Entities;
using TaskTracker.Services.Shared.Models;

namespace TaskTracker.Services.Tasks.ApplicationCore.Abstractions.Context;

public interface IApplicationDbContext
{
    DbSet<ApplicationUser> Users { get; set; }
    DbSet<TaskItem> Tasks { get; set; }
    
    DatabaseFacade Database { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
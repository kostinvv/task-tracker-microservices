using Microsoft.AspNetCore.Identity;
using TaskTracker.Services.Shared.Models;

namespace TaskTracker.Services.Shared.Data.Entities;

public class ApplicationUser : IdentityUser<Guid>
{
    public ICollection<TaskItem> TaskItems { get; set; } = new List<TaskItem>();
}
using Microsoft.AspNetCore.Identity;

namespace TaskTracker.Services.Shared.Models;

public class ApplicationUser : IdentityUser<Guid>
{
    public ICollection<TaskItem> TaskItems { get; set; } = new List<TaskItem>();
}
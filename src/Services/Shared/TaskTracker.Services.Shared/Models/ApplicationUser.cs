using Microsoft.AspNetCore.Identity;

namespace TaskTracker.Services.Shared.Models;

public class ApplicationUser : IdentityUser<Guid>
{
    public ICollection<TaskItem> TaskItems { get; private set; } = new List<TaskItem>();
}
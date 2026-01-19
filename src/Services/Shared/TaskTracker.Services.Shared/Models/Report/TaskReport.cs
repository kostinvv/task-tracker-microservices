using TaskTracker.Services.Shared.Data.Entities;

namespace TaskTracker.Services.Shared.Models.Report;

public class TaskReport
{
    public required string Title { get; init; }
    
    public required DateTime UpdatedAt { get; init; }

    public required TaskState TaskState { get; init; }
}
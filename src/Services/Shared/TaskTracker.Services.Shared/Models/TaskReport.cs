namespace TaskTracker.Services.Shared.Models;

public class TaskReport
{
    public required string Title { get; init; }

    public required DateTime CreatedAt { get; init; }
    
    public required DateTime UpdatedAt { get; init; }

    public required TaskState TaskState { get; init; }
}
namespace TaskTracker.Services.Shared.Models.Report;

public class UserReport
{
    public required string Email { get; init; }

    public required IEnumerable<TaskReport>  LastCompletedTasks { get; init; } = [];
    
    public required IEnumerable<TaskReport> UncompletedTasks { get; init; } = [];

    public required int CompletedTaskCount { get; init; }
    
    public required int UncompletedTaskCount { get; init; }
}
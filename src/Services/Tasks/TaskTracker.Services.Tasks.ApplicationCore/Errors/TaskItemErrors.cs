using TaskTracker.Services.Shared.Results;

namespace TaskTracker.Services.Tasks.ApplicationCore.Errors;

public abstract class TaskItemErrors
{
    public static Error Validation(string description) => 
        Error.Validation("TaskItem.Validation", description);
    
    public static Error Conflict(string description) =>
        Error.Conflict("TaskItem.Conflict", description);
    
    public static Error NotFound(string id) =>
        Error.NotFound("TaskItem.NotFound", $"Task with Id: {id} not found");
}
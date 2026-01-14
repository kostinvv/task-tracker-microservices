using TaskTracker.Services.Shared.Results;

namespace TaskTracker.Services.Tasks.ApplicationCore.Errors;

public class TaskItemErrors
{
    public static Error Validation(string description) => 
        Error.Validation("TaskItem.Validation", description);
    
    public static Error NotFound(string id) =>
        Error.NotFound("TaskItem.NotFound", $"Task with Id: {id} not found");
}
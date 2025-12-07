using TaskTracker.Services.Shared.Results;

namespace TaskTracker.Services.Tasks.ApplicationCore.Errors;

public class TaskItemErrors
{
    public static Error NotFound(string id) =>
        Error.NotFound("TaskItem.NotFound", $"Task with Id: {id} not found");
}
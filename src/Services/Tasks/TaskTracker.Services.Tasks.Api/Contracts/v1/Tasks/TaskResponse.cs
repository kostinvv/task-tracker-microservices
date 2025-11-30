using TaskTracker.Services.Tasks.ApplicationCore.Models;

namespace TaskTracker.Services.Tasks.Api.Contracts.v1.Tasks;

public record TaskResponse(
    Guid Id,
    string Title,
    string Description,
    TaskState State,
    int SortOrder,
    DateTime CreatedAt,
    DateTime UpdatedAt
)
{
    public static TaskResponse Map(TaskItem taskItem) =>
        new(
            taskItem.Id,
            taskItem.Title,
            taskItem.Description,
            taskItem.TaskState,
            taskItem.SortOrder,
            taskItem.CreatedAt,
            taskItem.UpdatedAt
        );
}
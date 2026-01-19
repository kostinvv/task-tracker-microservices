using TaskTracker.Services.Shared.Models;

namespace TaskTracker.Services.Tasks.ApplicationCore.DTOs.Tasks;

public record TaskDto(Guid? Id, string Title, string? Description, TaskState State, int SortOrder, Guid UserId)
{
    public TaskItem ToEntity()
    {
        return TaskItem.Create(            
            title: Title,
            description: Description,
            createdAt: DateTime.UtcNow,
            updatedAt: DateTime.UtcNow,
            userId: UserId,
            sortOrder: SortOrder,
            taskState: State);
    }

    public static TaskDto CreateDto(TaskItem taskItem)
    {
        return new TaskDto(
            taskItem.Id, 
            taskItem.Title, 
            null, 
            taskItem.TaskState, 
            taskItem.SortOrder, 
            taskItem.UserId);
    }

    public static TaskDto CreateWithDescription(TaskItem taskItem)
    {
        return new TaskDto(
            taskItem.Id, 
            taskItem.Title, 
            taskItem.Description, 
            taskItem.TaskState, 
            taskItem.SortOrder, 
            taskItem.UserId);
    }
}
using TaskTracker.Services.Tasks.ApplicationCore.Models;

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
}
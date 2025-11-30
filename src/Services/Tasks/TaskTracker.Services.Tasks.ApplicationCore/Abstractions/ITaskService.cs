using TaskTracker.Services.Tasks.ApplicationCore.Common.Results;
using TaskTracker.Services.Tasks.ApplicationCore.DTOs.Tasks;
using TaskTracker.Services.Tasks.ApplicationCore.Models;

namespace TaskTracker.Services.Tasks.ApplicationCore.Abstractions;

public interface ITaskService
{
    Task<OperationResult<IEnumerable<TaskItem>>> GetAllAsync(Guid userId, CancellationToken cancellationToken);
    
    Task<OperationResult<TaskItem?>> GetByIdAsync(Guid taskId, CancellationToken cancellationToken);
    
    Task<OperationResult<TaskItem>> CreateAsync(TaskDto taskDto, CancellationToken cancellationToken);

    Task<OperationResult<TaskItem>> UpdateAsync(TaskDto taskDto, CancellationToken cancellationToken);
    
    Task DeleteAsync(TaskItem taskItem, CancellationToken cancellationToken);
}
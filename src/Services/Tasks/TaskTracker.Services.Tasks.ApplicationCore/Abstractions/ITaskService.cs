using TaskTracker.Services.Shared.Results;
using TaskTracker.Services.Tasks.ApplicationCore.DTOs;
using TaskTracker.Services.Tasks.ApplicationCore.DTOs.Tasks;
using TaskTracker.Services.Tasks.ApplicationCore.Models;

namespace TaskTracker.Services.Tasks.ApplicationCore.Abstractions;

public interface ITaskService
{
    Task<ResultT<CursorList<TaskDto>>> GetAsync(Guid userId, int afterPosition, int size, TaskState state, CancellationToken cancellationToken);
    
    Task<ResultT<IEnumerable<TaskListDto>>> GetBoardAsync(Guid userId, int size, CancellationToken cancellationToken);
    
    Task<ResultT<TaskItem>> GetByIdAsync(Guid taskId, Guid userId, CancellationToken cancellationToken);
    
    Task<ResultT<TaskItem>> CreateAsync(TaskDto taskDto, CancellationToken cancellationToken);

    Task<ResultT<TaskItem>> UpdateAsync(TaskDto taskDto, CancellationToken cancellationToken);
    
    Task<Result> DeleteAsync(Guid id, Guid userId, CancellationToken cancellationToken);
    
    Task<Result> MoveAsync(Guid taskId, Guid userId, int nextOrder, TaskState state, CancellationToken cancellationToken); 
}
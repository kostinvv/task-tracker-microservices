using Microsoft.EntityFrameworkCore;
using TaskTracker.Services.Tasks.ApplicationCore.Abstractions;
using TaskTracker.Services.Tasks.ApplicationCore.Abstractions.Context;
using TaskTracker.Services.Tasks.ApplicationCore.Common.Results;
using TaskTracker.Services.Tasks.ApplicationCore.DTOs.Tasks;
using TaskTracker.Services.Tasks.ApplicationCore.Models;

namespace TaskTracker.Services.Tasks.ApplicationCore.Services;

public class TaskService(IApplicationDbContext context) : ITaskService
{
    public async Task<OperationResult<IEnumerable<TaskItem>>> GetAllAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await context.Tasks
            .Where(taskItem => taskItem.UserId == userId)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
    
    public async Task<OperationResult<TaskItem?>> GetByIdAsync(Guid taskId, CancellationToken cancellationToken)
    {
        return await context.Tasks
            .AsNoTracking()
            .FirstOrDefaultAsync(taskItem => taskItem.Id == taskId, cancellationToken);
    }

    public async Task<OperationResult<TaskItem>> CreateAsync(TaskDto taskDto, CancellationToken cancellationToken)
    {
        var taskItem = TaskItem.Create(
            title: taskDto.Title,
            description: taskDto.Description,
            createdAt: DateTime.UtcNow,
            updatedAt: DateTime.UtcNow,
            userId: taskDto.UserId,
            sortOrder:  taskDto.SortOrder,
            taskState: taskDto.State
        );
        
        await context.Tasks.AddAsync(taskItem, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        
        return taskItem;
    }

    public async Task<OperationResult<TaskItem>> UpdateAsync(TaskDto taskDto, CancellationToken cancellationToken)
    {
        var taskItem = await context.Tasks
            .FirstOrDefaultAsync(taskItem => taskItem.Id == taskDto.Id, cancellationToken);

        if (taskItem is null || taskItem.UserId != taskDto.UserId)
        {
            return Error.RecordNotFound("Task not found.");
        }

        taskItem.Update(
            title: taskDto.Title,
            description: taskDto.Description,
            state: taskDto.State,
            sortOrder: taskDto.SortOrder,
            updatedAt: DateTime.UtcNow
        );
        await context.SaveChangesAsync(cancellationToken);
        return taskItem;
    }

    public async Task DeleteAsync(TaskItem taskItem, CancellationToken cancellationToken)
    {
        context.Tasks.Remove(taskItem);
        await context.SaveChangesAsync(cancellationToken);
    }
}
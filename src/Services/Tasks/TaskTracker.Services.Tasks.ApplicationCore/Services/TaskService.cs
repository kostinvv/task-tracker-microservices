using Microsoft.EntityFrameworkCore;
using TaskTracker.Services.Shared.Results;
using TaskTracker.Services.Tasks.ApplicationCore.Abstractions;
using TaskTracker.Services.Tasks.ApplicationCore.Abstractions.Context;
using TaskTracker.Services.Tasks.ApplicationCore.DTOs.Tasks;
using TaskTracker.Services.Tasks.ApplicationCore.Errors;
using TaskTracker.Services.Tasks.ApplicationCore.Models;

namespace TaskTracker.Services.Tasks.ApplicationCore.Services;

public class TaskService(IApplicationDbContext context) : ITaskService
{
    public async Task<ResultT<IEnumerable<TaskItem>>> GetAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await context.Tasks
            .Where(taskItem => taskItem.UserId == userId)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
    
    public async Task<ResultT<TaskItem>> GetByIdAsync(Guid taskId, Guid userId, CancellationToken cancellationToken)
    {
        var taskItem = await context.Tasks
            .AsNoTracking()
            .FirstOrDefaultAsync(taskItem => taskItem.Id == taskId && taskItem.UserId == userId, cancellationToken);

        if (taskItem == null)
        {
            return TaskItemErrors.NotFound(taskId.ToString());
        }
        
        return taskItem;
    }

    public async Task<ResultT<TaskItem>> CreateAsync(TaskDto taskDto, CancellationToken cancellationToken)
    {
        var taskItem = taskDto.ToEntity();
        await context.Tasks.AddAsync(taskItem, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return taskItem;
    }

    public async Task<ResultT<TaskItem>> UpdateAsync(TaskDto taskDto, CancellationToken cancellationToken)
    {
        var taskItem = await context.Tasks.FirstOrDefaultAsync(
            taskItem => taskItem.Id == taskDto.Id && 
            taskItem.UserId == taskDto.UserId, 
            cancellationToken: cancellationToken);

        if (taskItem == null)
        {
            return TaskItemErrors.NotFound(taskDto.Id.ToString());
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

    public async Task<Result> DeleteAsync(Guid id, Guid userId, CancellationToken cancellationToken)
    {
        var taskItem = await context.Tasks.FirstOrDefaultAsync(
            taskItem => taskItem.Id == id && taskItem.UserId == userId, cancellationToken: cancellationToken);

        if (taskItem == null)
        {
            return TaskItemErrors.NotFound(id.ToString());
        }
        
        context.Tasks.Remove(taskItem);
        await context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
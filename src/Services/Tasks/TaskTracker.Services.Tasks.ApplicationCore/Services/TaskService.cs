using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaskTracker.Services.Shared.Results;
using TaskTracker.Services.Tasks.ApplicationCore.Abstractions;
using TaskTracker.Services.Tasks.ApplicationCore.Abstractions.Context;
using TaskTracker.Services.Tasks.ApplicationCore.DTOs;
using TaskTracker.Services.Tasks.ApplicationCore.DTOs.Tasks;
using TaskTracker.Services.Tasks.ApplicationCore.Errors;
using TaskTracker.Services.Tasks.ApplicationCore.Extensions;
using TaskTracker.Services.Tasks.ApplicationCore.Models;

namespace TaskTracker.Services.Tasks.ApplicationCore.Services;

public class TaskService(
    IApplicationDbContext context, 
    ILogger<TaskService> logger) : ITaskService
{
    public async Task<ResultT<CursorList<TaskDto>>> GetAsync(
        Guid userId, 
        int afterPosition, 
        int size, 
        TaskState state, 
        CancellationToken cancellationToken)
    {
        var query = context.Tasks
            .AsNoTracking()
            .OrderBy(taskItem => taskItem.SortOrder)
            .Where(taskItem => 
                taskItem.UserId == userId && 
                taskItem.TaskState == state &&
                taskItem.SortOrder > afterPosition)
            .Select(taskItem => new TaskDto(
                taskItem.Id, 
                taskItem.Title, 
                taskItem.Description, 
                taskItem.TaskState, 
                taskItem.SortOrder, 
                taskItem.UserId));
        
        return await CursorList<TaskDto>.CreateAsync(query, size);
    }

    public async Task<ResultT<IEnumerable<TaskListDto>>> GetBoardAsync(
        Guid userId, 
        int size, 
        CancellationToken cancellationToken)
    {
        var result = new List<TaskListDto>();
        var states = Enum.GetValues<TaskState>().OrderBy(state => state);

        foreach (var state in states)
        {
            var query = context.Tasks
                .AsNoTracking()
                .OrderBy(taskItem => taskItem.SortOrder)
                .Where(taskItem =>
                    taskItem.UserId == userId &&
                    taskItem.TaskState == state)
                .Select(taskItem => new TaskDto(
                    taskItem.Id,
                    taskItem.Title,
                    taskItem.Description,
                    taskItem.TaskState,
                    taskItem.SortOrder,
                    taskItem.UserId));

            var pagedList = await CursorList<TaskDto>
                .CreateAsync(query, size);
            
            result.Add(new TaskListDto(id: state, title: state.GetDisplayName(), cursorList: pagedList));
        }

        return result;
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
        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
        
        try
        {
            await context.Tasks
                .Where(item => item.UserId == taskDto.UserId && item.TaskState == taskDto.State)
                .ExecuteUpdateAsync(
                    c => c.SetProperty(item => item.SortOrder, item => item.SortOrder + 1), 
                    cancellationToken);

            var taskItem = taskDto.ToEntity();
            await context.Tasks.AddAsync(taskItem, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return taskItem;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<ResultT<TaskItem>> UpdateAsync(Guid taskId, TaskDto taskDto, CancellationToken cancellationToken)
    {
        var taskItem = await context.Tasks.FirstOrDefaultAsync(
            taskItem => taskItem.Id == taskId && 
            taskItem.UserId == taskDto.UserId, 
            cancellationToken: cancellationToken);

        if (taskItem == null)
        {
            return TaskItemErrors.NotFound(taskId.ToString());
        }
        
        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
        
        try
        {
            var sortOrder = taskItem.SortOrder;
            
            if (taskItem.TaskState != taskDto.State)
            {
                await context.Tasks
                    .Where(item => item.UserId == taskDto.UserId && item.TaskState == taskDto.State)
                    .ExecuteUpdateAsync(
                        c => c.SetProperty(item => item.SortOrder, item => item.SortOrder + 1), 
                        cancellationToken);

                sortOrder = 0;
            }
            
            taskItem.Update(
                title: taskDto.Title,
                description: taskDto.Description,
                state: taskDto.State,
                sortOrder: sortOrder,
                updatedAt: DateTime.UtcNow
            );
            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return taskItem;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<Result> DeleteAsync(Guid id, Guid userId, CancellationToken cancellationToken)
    {
        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var taskItem = await context.Tasks.FirstOrDefaultAsync(
                taskItem => taskItem.Id == id && taskItem.UserId == userId, cancellationToken: cancellationToken);

            if (taskItem == null)
            {
                return TaskItemErrors.NotFound(id.ToString());
            }
            
            await context.Tasks
                .Where(item => 
                    item.UserId == userId && 
                    item.TaskState == taskItem.TaskState &&
                    item.SortOrder > taskItem.SortOrder)
                .ExecuteUpdateAsync(
                    c => c.SetProperty(item => item.SortOrder, item => item.SortOrder - 1), 
                    cancellationToken);   
        
            context.Tasks.Remove(taskItem);
            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return Result.Success();
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<Result> MoveAsync(Guid taskId, Guid userId, int nextOrder, TaskState state, CancellationToken cancellationToken)
    {
        await using var transaction = await context.Database
            .BeginTransactionAsync(cancellationToken);

        try
        {
            var taskItem = await context.Tasks
                .FirstOrDefaultAsync(taskItem => 
                        taskItem.Id == taskId && 
                        taskItem.UserId == userId, 
                    cancellationToken: cancellationToken);

            if (taskItem == null)
            {
                return TaskItemErrors.NotFound(taskId.ToString());
            }

            if (state != taskItem.TaskState)
            {
                await MoveToStateAsync(taskItem, state, nextOrder, cancellationToken);
            }
            else
            {
                await MoveAndReorderAsync(taskItem, nextOrder, cancellationToken);    
            }
            
            await transaction.CommitAsync(cancellationToken);
            return Result.Success();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{Message}", ex.Message);
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    private async Task MoveAndReorderAsync(TaskItem taskItem, int nextOrder, CancellationToken cancellationToken)
    {
        var prevOrder = taskItem.SortOrder;
        
        if (nextOrder > prevOrder)
        {   
            await context.Tasks
                .Where(item => 
                    item.UserId == taskItem.UserId &&
                    item.TaskState == taskItem.TaskState &&
                    item.SortOrder > prevOrder && 
                    item.SortOrder <= nextOrder)
                .ExecuteUpdateAsync(
                    c => c.SetProperty(item => item.SortOrder, item => item.SortOrder - 1),
                    cancellationToken);
        }
        else if (nextOrder < prevOrder)
        {
            await context.Tasks
                .Where(item => 
                    item.UserId == taskItem.UserId &&
                    item.TaskState == taskItem.TaskState &&
                    item.SortOrder < prevOrder && 
                    item.SortOrder >= nextOrder)
                .ExecuteUpdateAsync(
                    c => c.SetProperty(item => item.SortOrder, item => item.SortOrder + 1),
                    cancellationToken);
        }
        
        taskItem.SetSortOrder(nextOrder);
        await context.SaveChangesAsync(cancellationToken);
    }

    private async Task MoveToStateAsync(TaskItem taskItem, TaskState nextState, int nextOrder, CancellationToken cancellationToken)
    {
        var prevState = taskItem.TaskState;
        var prevOrder = taskItem.SortOrder;

        await context.Tasks
            .Where(item => 
                item.UserId == taskItem.UserId &&
                item.TaskState == prevState &&
                item.SortOrder > prevOrder)
            .ExecuteUpdateAsync(
                c => c.SetProperty(item => item.SortOrder, item => item.SortOrder - 1),
                cancellationToken);
        
        await context.Tasks
            .Where(item => 
                item.UserId == taskItem.UserId &&
                item.TaskState == nextState &&
                item.SortOrder >= nextOrder)
            .ExecuteUpdateAsync(
                c => c.SetProperty(item => item.SortOrder, item => item.SortOrder + 1),
                cancellationToken);
        
        taskItem.SetState(nextState);
        taskItem.SetSortOrder(nextOrder);
        await context.SaveChangesAsync(cancellationToken);
    }
}
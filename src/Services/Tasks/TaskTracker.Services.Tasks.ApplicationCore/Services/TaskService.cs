using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaskTracker.Services.Shared.Data.Entities;
using TaskTracker.Services.Shared.Models;
using TaskTracker.Services.Shared.Results;
using TaskTracker.Services.Tasks.ApplicationCore.Abstractions;
using TaskTracker.Services.Tasks.ApplicationCore.Abstractions.Context;
using TaskTracker.Services.Tasks.ApplicationCore.DTOs;
using TaskTracker.Services.Tasks.ApplicationCore.DTOs.Tasks;
using TaskTracker.Services.Tasks.ApplicationCore.Errors;
using TaskTracker.Services.Tasks.ApplicationCore.Extensions;

namespace TaskTracker.Services.Tasks.ApplicationCore.Services;

public class TaskService(
    IApplicationDbContext context,
    ITaskOrderService taskOrderService,
    ILogger<TaskService> logger) : ITaskService
{
    public async Task<ResultT<PagedList<TaskDto>>> GetAsync(
        Guid userId, 
        int skip, 
        int size, 
        TaskState state, 
        CancellationToken cancellationToken)
    {
        var query = context.Tasks
            .AsNoTracking()
            .OrderBy(taskItem => taskItem.SortOrder)
            .Where(taskItem => taskItem.UserId == userId && taskItem.TaskState == state)
            .Select(taskItem => new TaskDto(
                taskItem.Id,
                taskItem.Title,
                null,
                taskItem.TaskState,
                taskItem.SortOrder,
                taskItem.UserId));
        
        return await PagedList<TaskDto>.CreateAsync(query, size, skip, cancellationToken);
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
                    null,
                    taskItem.TaskState,
                    taskItem.SortOrder,
                    taskItem.UserId));

            var pagedList = await PagedList<TaskDto>
                .CreateAsync(query, size, skip: 0, cancellationToken);
            
            result.Add(new TaskListDto(id: state, title: state.GetDisplayName(), pagedList: pagedList));
        }

        return result;
    }

    public async Task<ResultT<TaskDto>> GetByIdAsync(Guid taskId, Guid userId, CancellationToken cancellationToken)
    {
        var taskItem = await context.Tasks
            .AsNoTracking()
            .FirstOrDefaultAsync(taskItem => taskItem.Id == taskId && taskItem.UserId == userId, cancellationToken);

        if (taskItem == null)
        {
            return TaskItemErrors.NotFound(taskId.ToString());
        }

        return TaskDto.CreateWithDescription(taskItem);
    }

    public async Task<ResultT<TaskDto>> CreateAsync(TaskDto taskDto, CancellationToken cancellationToken)
    {
        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
        
        try
        {
            await taskOrderService.UpdateSortOrderDownAsync(
                userId: taskDto.UserId,
                state: taskDto.State,
                minOrderExclusive: taskDto.SortOrder,
                maxOrderExclusive: null,
                ct: cancellationToken);
            
            var taskItem = taskDto.ToEntity();
            await context.Tasks.AddAsync(taskItem, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return TaskDto.CreateDto(taskItem);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка при создании новой задачи: {TaskDto}", taskDto);
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<ResultT<TaskDto>> UpdateAsync(Guid taskId, TaskDto taskDto, CancellationToken cancellationToken)
    {
        var taskItem = await context.Tasks
            .FirstOrDefaultAsync(taskItem => 
                taskItem.Id == taskId && 
                taskItem.UserId == taskDto.UserId, 
                cancellationToken: cancellationToken);

        if (taskItem == null)
        {
            return TaskItemErrors.NotFound(
                taskId.ToString()
            );
        }
        
        taskItem.Title = taskDto.Title;
        taskItem.Description = taskDto.Description;

        await context.SaveChangesAsync(cancellationToken);
        return TaskDto.CreateDto(taskItem);
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
            
            context.Tasks.Remove(taskItem);
            await context.SaveChangesAsync(cancellationToken);

            await taskOrderService.UpdateSortOrderUpAsync(
                userId: userId,
                state: taskItem.TaskState,
                minOrderExclusive: taskItem.SortOrder,
                maxOrderExclusive: null,
                ct: cancellationToken);
            
            await transaction.CommitAsync(cancellationToken);
            return Result.Success();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка при удалении задачи. Task ID: {TaskId}, User ID: {UserId}", id, userId);
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<Result> MoveAsync(Guid taskId, Guid userId, int newOrder, TaskState newState, CancellationToken cancellationToken)
    {
        await using var transaction = await context.Database
            .BeginTransactionAsync(cancellationToken);

        try
        {
            var taskItem = await context.Tasks
                .FirstOrDefaultAsync(item => item.Id == taskId && item.UserId == userId, 
                    cancellationToken: cancellationToken);
            
            if (taskItem is null)
            {
                return TaskItemErrors.NotFound(
                    id: taskId.ToString()
                );
            }

            if (newState == taskItem.TaskState)
            {
                await taskOrderService.UpdateSortOrderRangeAsync(
                    userId: userId,
                    state: taskItem.TaskState,
                    oldOrder: taskItem.SortOrder,
                    newOrder: newOrder,
                    cancellationToken);
            }
            else
            {
                await taskOrderService.UpdateSortOrderUpAsync(
                    userId: userId,
                    state: taskItem.TaskState,
                    minOrderExclusive: taskItem.SortOrder,
                    maxOrderExclusive: null,
                    ct: cancellationToken);

                await taskOrderService.UpdateSortOrderDownAsync(
                    userId: userId,
                    state: newState,
                    minOrderExclusive: newOrder,
                    maxOrderExclusive: null,
                    ct: cancellationToken);
                
                taskItem.TaskState = newState;
            }
            taskItem.SortOrder = newOrder;
            await context.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);
            return Result.Success();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, 
                "Ошибка при перемещении задачи. Task ID: {TaskId}, User ID: {UserId}, NextOrder: {NextOrder}, NextState: {NextState}", 
                taskId, 
                userId, 
                newOrder,
                newState);
            
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
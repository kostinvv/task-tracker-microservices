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
    public async Task<ResultT<PagedList<TaskDto>>> GetAsync(
        Guid userId, 
        int page, 
        int size, 
        TaskState state, 
        CancellationToken cancellationToken)
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
        
        return await PagedList<TaskDto>.CreateAsync(query, page, size);
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

            var pagedList = await PagedList<TaskDto>
                .CreateAsync(query, pageSize: size);
            
            result.Add(new TaskListDto(id: state, title: state.GetDisplayName(), pagedList: pagedList));
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

    public async Task<Result> MoveAsync(Guid taskId, Guid userId, int nextOrder, TaskState state, CancellationToken cancellationToken)
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
        await using var tx = await context.Database.BeginTransactionAsync(cancellationToken);
        logger.LogInformation(
            "From: {From}, To: {To}, State: {State}, NewState: {NewState}", 
            taskItem.SortOrder, 
            nextOrder, 
            taskItem.TaskState, 
            state);

        if (state != taskItem.TaskState)
        {
            await MoveToStateAsync(taskItem, state, nextOrder, cancellationToken);
        }
        else
        {
            await MoveAndReorderAsync(taskItem, nextOrder, cancellationToken);    
        }
        await tx.CommitAsync(cancellationToken);
        return Result.Success();
    }

    private async Task MoveAndReorderAsync(TaskItem taskItem, int nextOrder, CancellationToken cancellationToken)
    {
        var prevOrder = taskItem.SortOrder;
        
        if (nextOrder > prevOrder)
        {   
            await context.Tasks
                .Where(item => 
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
                item.TaskState == prevState &&
                item.SortOrder > prevOrder)
            .ExecuteUpdateAsync(
                c => c.SetProperty(item => item.SortOrder, item => item.SortOrder - 1),
                cancellationToken);
        
        await context.Tasks
            .Where(item => 
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
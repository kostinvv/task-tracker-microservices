using Microsoft.EntityFrameworkCore;
using TaskTracker.Services.Tasks.ApplicationCore.Abstractions;
using TaskTracker.Services.Tasks.ApplicationCore.Abstractions.Context;
using TaskTracker.Services.Tasks.ApplicationCore.Models;

namespace TaskTracker.Services.Tasks.ApplicationCore.Services;

public class TaskOrderService(
    IApplicationDbContext context) : ITaskOrderService
{
    public async Task UpdateSortOrderRangeAsync(
        Guid userId, 
        TaskState state, 
        int oldOrder, 
        int newOrder, 
        CancellationToken ct)
    {
        if (oldOrder == newOrder)
        {
            return;
        }

        if (newOrder > oldOrder)
        {
            await UpdateSortOrderUpAsync(
                userId: userId,
                state: state,
                minOrderExclusive: oldOrder,
                maxOrderExclusive: newOrder,
                ct);
        }
        else
        {
            await UpdateSortOrderDownAsync(
                userId: userId,
                state: state,
                minOrderExclusive: newOrder,
                maxOrderExclusive: oldOrder,
                ct);
        }
    }

    public async Task UpdateSortOrderUpAsync(
        Guid userId, 
        TaskState state, 
        int minOrderExclusive,
        int? maxOrderExclusive,
        CancellationToken ct)
    {
        var query = context.Tasks
            .Where(item => item.UserId == userId && 
                           item.TaskState == state && 
                           item.SortOrder > minOrderExclusive);

        if (maxOrderExclusive.HasValue) 
        {
            query = query.Where(item => item.SortOrder <= maxOrderExclusive.Value);
        }

        await query.ExecuteUpdateAsync(
            c => c.SetProperty(item => item.SortOrder, item => item.SortOrder - 1),
            cancellationToken: ct);
    }

    public async Task UpdateSortOrderDownAsync(
        Guid userId, 
        TaskState state, 
        int minOrderExclusive,
        int? maxOrderExclusive,
        CancellationToken ct)
    {
        var query = context.Tasks
            .Where(item => item.UserId == userId && 
                           item.TaskState == state && 
                           item.SortOrder >= minOrderExclusive);

        if (maxOrderExclusive.HasValue) 
        {
            query = query.Where(item => item.SortOrder < maxOrderExclusive.Value);
        }

        await query.ExecuteUpdateAsync(
            c => c.SetProperty(item => item.SortOrder, item => item.SortOrder + 1),
            cancellationToken: ct);
    }
}
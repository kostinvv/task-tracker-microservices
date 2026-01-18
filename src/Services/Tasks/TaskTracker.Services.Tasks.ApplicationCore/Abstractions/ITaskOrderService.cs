using TaskTracker.Services.Tasks.ApplicationCore.Models;

namespace TaskTracker.Services.Tasks.ApplicationCore.Abstractions;

public interface ITaskOrderService
{
    public Task UpdateSortOrderRangeAsync(
        Guid userId, 
        TaskState state, 
        int oldOrder, 
        int newOrder, 
        CancellationToken ct);
    
    public Task UpdateSortOrderUpAsync(
        Guid userId, 
        TaskState state, 
        int minOrderExclusive,
        int? maxOrderExclusive,
        CancellationToken ct);
    
    public Task UpdateSortOrderDownAsync(
        Guid userId, 
        TaskState state, 
        int minOrderExclusive,
        int? maxOrderExclusive,
        CancellationToken ct);
}
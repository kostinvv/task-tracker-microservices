using Microsoft.EntityFrameworkCore;

namespace TaskTracker.Services.Tasks.ApplicationCore.DTOs;

public class PagedList<T>
{
    private PagedList(List<T> items, bool hasNextPage)
    {
        Items = items;
        HasNextPage = hasNextPage;
    }

    public List<T> Items { get; private set; }
    public bool HasNextPage { get; private set; }
    
    public static async Task<PagedList<T>> CreateAsync(IQueryable<T> query, int size, int skip, CancellationToken ct)
    {
        var pagedItems = await query
            .Skip(skip)
            .Take(size + 1)
            .ToListAsync(ct);
        
        var hasNextPage = pagedItems.Count > size;
        var items = pagedItems.Take(size).ToList();
        
        return new PagedList<T>(items, hasNextPage);
    }
}
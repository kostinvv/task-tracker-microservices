using Microsoft.EntityFrameworkCore;

namespace TaskTracker.Services.Tasks.ApplicationCore.DTOs;

public class CursorList<TItem>
{
    private CursorList(List<TItem> items, bool hasNextPage)
    {
        Items = items;
        HasNextPage = hasNextPage;
    }

    public List<TItem> Items { get; private set; }
    
    public bool HasNextPage { get; private set; }
    
    public static async Task<CursorList<TItem>> CreateAsync(IQueryable<TItem> query, int size)
    {
        var pagedItems = await query
            .Take(size + 1)
            .ToListAsync();
        
        var hasNextPage = pagedItems.Count > size;
        var items = pagedItems.Take(size).ToList();
        
        return new CursorList<TItem>(items, hasNextPage);
    }
}
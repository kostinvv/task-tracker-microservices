using Microsoft.EntityFrameworkCore;

namespace TaskTracker.Services.Tasks.ApplicationCore.DTOs;

public class PagedList<TItem>
{
    private PagedList(List<TItem> items, int page, int pageSize, int totalCount)
    {
        Items = items;
        Page = page;
        PageSize = pageSize;
        TotalCount = totalCount;
    }

    public List<TItem> Items { get; }

    public int Page { get; }
    
    public int PageSize { get; }
    
    public int TotalCount { get; }
    
    public bool HasNextPage => Page * PageSize < TotalCount;
    
    public bool HasPreviousPage => PageSize > 1;

    public static async Task<PagedList<TItem>> CreateAsync(IQueryable<TItem> query, int page = 1, int pageSize = 10)
    {
        var totalCount = await query.CountAsync();
        
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        
        return new PagedList<TItem>(items, page, pageSize, totalCount);
    }
}
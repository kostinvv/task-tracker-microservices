using TaskTracker.Services.Scheduler.Abstractions;
using TaskTracker.Services.Shared.Data.Entities;
using TaskTracker.Services.Shared.Models.Report;

namespace TaskTracker.Services.Scheduler.Services;

public class UserReportService(SchedulerDbContext context) : IUserReportService
{
    public async Task<IEnumerable<UserReport>> GetUserReportsAsync()
    {
        return await context.Users
            .AsNoTracking()
            .AsSplitQuery()
            .Select(user => new UserReport
            {
                Email = user.Email!,
                UncompletedTaskCount = user.TaskItems
                    .Count(item => item.TaskState == TaskState.ToDo || item.TaskState == TaskState.InProgress),
                CompletedTaskCount = user.TaskItems
                    .Count(item => item.TaskState == TaskState.Done && item.UpdatedAt >= DateTime.UtcNow.AddDays(-1)),
                UncompletedTasks = user.TaskItems
                    .Where(item => item.TaskState == TaskState.ToDo || item.TaskState == TaskState.InProgress)
                    .OrderBy(item => item.UpdatedAt)
                    .Take(3)
                    .Select(item => new TaskReport
                    {
                        Title = item.Title,
                        UpdatedAt = item.UpdatedAt,
                        TaskState = item.TaskState
                    }),
                LastCompletedTasks = user.TaskItems
                    .Where(item => item.TaskState == TaskState.Done && item.UpdatedAt >= DateTime.UtcNow.AddDays(-1))
                    .OrderByDescending(item => item.UpdatedAt)
                    .Take(3)
                    .Select(item => new TaskReport
                    {
                        Title = item.Title,
                        UpdatedAt = item.UpdatedAt,
                        TaskState = item.TaskState
                    })
            })
            .ToListAsync();
    }
}
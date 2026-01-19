using TaskTracker.Services.Shared.Models.Report;

namespace TaskTracker.Services.Scheduler.Abstractions;

public interface IUserReportService
{
    public Task<IEnumerable<UserReport>> GetUserReportsAsync();
}
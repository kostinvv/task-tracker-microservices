namespace TaskTracker.Services.Scheduler.Options;

public class SchedulerOptions
{
    public const string SectionName = "Quartz";

    public required string CronSchedule { get; init; }
}
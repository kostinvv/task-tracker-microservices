using TaskTracker.Services.Shared.Data.Entities;
using TaskTracker.Services.Shared.Emails;
using TaskTracker.Services.Shared.Kafka;
using TaskTracker.Services.Shared.Models.Report;

namespace TaskTracker.Services.Scheduler;

public class Worker(
    SchedulerDbContext schedulerDbContext,
    IEmailTemplateService emailTemplateService,
    IKafkaProducer<EmailNotificationEvent> producer) : IJob
{
    public async Task Execute(IJobExecutionContext jobExecutionContext)
    {
        var userReports = await schedulerDbContext.Users
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
        
        var subject = EmailTemplate.Report.GetSubjectFromResource();

        foreach (var userReport in userReports)
        {
            var reportBody = await emailTemplateService.GetEmailBodyAsync(
                EmailTemplate.Report, model: userReport);

            var message = EmailNotificationEvent.Create(
                email: userReport.Email,
                subject: subject!,
                body: reportBody!);
            
            await producer.ProduceAsync(
                key: Guid.NewGuid().ToString(), 
                message, 
                cancellationToken: CancellationToken.None);
        } 
    }
}
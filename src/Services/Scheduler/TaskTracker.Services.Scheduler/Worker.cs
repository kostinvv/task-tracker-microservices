using TaskTracker.Services.Scheduler.Abstractions;
using TaskTracker.Services.Shared.Emails;
using TaskTracker.Services.Shared.Kafka;

namespace TaskTracker.Services.Scheduler;

public class Worker(
    IUserReportService userReportService,
    IEmailTemplateService emailTemplateService,
    IKafkaProducer<EmailNotificationEvent> producer) : IJob
{
    public async Task Execute(IJobExecutionContext jobExecutionContext)
    {
        var userReports = await userReportService.GetUserReportsAsync();
        var subject = EmailTemplate.Report.GetSubjectFromResource()!;
        
        foreach (var userReport in userReports)
        {
            var reportBody = await emailTemplateService.GetEmailBodyAsync(
                emailTemplate: EmailTemplate.Report, 
                model: userReport);

            var message = EmailNotificationEvent.Create(
                email: userReport.Email,
                subject: subject,
                body: reportBody!);
            
            await producer.ProduceAsync(
                key: Guid.NewGuid().ToString(), 
                message: message, 
                cancellationToken: CancellationToken.None);
        } 
    }
}
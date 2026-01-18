namespace TaskTracker.Services.Shared.Emails;

public interface IEmailTemplateService
{
    public Task<string?> GetEmailBodyAsync(EmailTemplate emailTemplate, object? model);
}

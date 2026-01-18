using TaskTracker.Services.EmailSender.DTOs;
using TaskTracker.Services.Shared.Results;

namespace TaskTracker.Services.EmailSender.Abstractions;

public interface IEmailService
{
    public Task<Result> SendEmailAsync(
        EmailDto emailDto, 
        CancellationToken cancellationToken);
}
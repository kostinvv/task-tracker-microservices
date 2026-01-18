using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using TaskTracker.Services.EmailSender.Abstractions;
using TaskTracker.Services.EmailSender.DTOs;
using TaskTracker.Services.EmailSender.Options;
using TaskTracker.Services.Shared.Results;

namespace TaskTracker.Services.EmailSender.Services;

public class EmailService(IOptions<SmtpOptions> smtpOptions) : IEmailService
{
    private readonly SmtpOptions _smtpOption = smtpOptions.Value;
    
    public async Task<Result> SendEmailAsync(EmailDto emailDto, CancellationToken cancellationToken)
    {
        var mimeMessage = new MimeMessage();
        
        mimeMessage.From.Add(
            MailboxAddress.Parse(text: _smtpOption.UserName)
        );
        mimeMessage.To.Add(
            MailboxAddress.Parse(text: emailDto.Email)
        );
        mimeMessage.Subject = emailDto.Subject;
        
        mimeMessage.Body = new TextPart(TextFormat.Html)
        {
            Text = emailDto.Body
        };

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(
            host: _smtpOption.Host, 
            port: _smtpOption.Port,
            SecureSocketOptions.StartTls,
            cancellationToken);

        await smtp.AuthenticateAsync(
            userName: _smtpOption.UserName, 
            password: _smtpOption.Password, 
            cancellationToken);
        
        await smtp.SendAsync(message: mimeMessage, cancellationToken);
        await smtp.DisconnectAsync(quit: true, cancellationToken);
        return Result.Success();
    }
}
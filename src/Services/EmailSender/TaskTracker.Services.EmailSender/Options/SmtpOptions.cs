namespace TaskTracker.Services.EmailSender.Options;

public class SmtpOptions
{
    public const string SectionName = "Smtp";

    public string Host { get; init; } = null!;

    public int Port { get; init; }

    public string UserName { get; init; } = null!;
    
    public string Password { get; init; } = null!;
}
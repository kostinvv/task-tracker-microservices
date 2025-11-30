namespace TaskTracker.Services.Tasks.ApplicationCore.Common.Results;

public record Error(ErrorCode Code, string Description)
{
    public static readonly Error None = new (ErrorCode.None, string.Empty);
    
    public static Error AuthenticationError(string description) => new (ErrorCode.AuthenticationError, description);
    public static Error RecordNotFound(string description) => new (ErrorCode.NotFoundError, description);
    public static Error ValidationError(string description) => new (ErrorCode.ValidationError, description);
}
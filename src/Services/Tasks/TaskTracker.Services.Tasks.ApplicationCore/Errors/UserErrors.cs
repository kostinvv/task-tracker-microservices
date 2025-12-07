using TaskTracker.Services.Shared.Results;

namespace TaskTracker.Services.Tasks.ApplicationCore.Errors;

public class UserErrors
{
    public static Error NotFound(string email) =>
        Error.NotFound("User.NotFound", $"User with E-mail: {email} not found");
    
    public static Error CreateFailure(string description) =>
        Error.Validation("User.CreateFailure", description);

    public static Error InvalidCredentials => 
        Error.AccessUnAuthorized("User.Unauthorized", "Invalid user or password");
}
namespace TaskTracker.Services.Shared.Events.Users;

public record UserRegisteredEvent(Guid UserId, string Email);
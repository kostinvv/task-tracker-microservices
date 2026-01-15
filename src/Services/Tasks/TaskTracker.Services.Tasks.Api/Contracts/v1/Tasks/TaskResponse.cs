using TaskTracker.Services.Tasks.ApplicationCore.Models;

namespace TaskTracker.Services.Tasks.Api.Contracts.v1.Tasks;

public record TaskResponse(
    Guid Id,
    string Title,
    TaskState State,
    int SortOrder,
    string Description = ""
);
using TaskTracker.Services.Tasks.ApplicationCore.Models;

namespace TaskTracker.Services.Tasks.Api.Contracts.v1.Tasks;

public record TaskRequest(Guid Id, string Title, string Description, TaskState State, int SortOrder);
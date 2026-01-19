using TaskTracker.Services.Shared.Models;

namespace TaskTracker.Services.Tasks.Api.Contracts.v1.Tasks;

public record TaskMoveRequest(int NewOrder, TaskState NewState);
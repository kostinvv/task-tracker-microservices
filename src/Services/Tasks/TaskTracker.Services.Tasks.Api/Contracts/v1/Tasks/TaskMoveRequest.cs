using TaskTracker.Services.Tasks.ApplicationCore.Models;

namespace TaskTracker.Services.Tasks.Api.Contracts.v1.Tasks;

public record TaskMoveRequest(
    int NextOrder, 
    TaskState NewState);
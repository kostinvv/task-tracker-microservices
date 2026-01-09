using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskTracker.Services.Shared.Results;
using TaskTracker.Services.Tasks.Api.Contracts.v1.Tasks;
using TaskTracker.Services.Tasks.ApplicationCore.Abstractions;

namespace TaskTracker.Services.Tasks.Api.Controllers.v1;

[Authorize]
[ApiController]
[ApiVersion(1)]
[Route("api/v{version:apiVersion}/tasks")]
public class TasksController(ITaskService taskService) : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAsync(CancellationToken cancellationToken = default)
    {
        var result = await taskService.GetAsync(userId: UserId, cancellationToken);
        
        return result.Match(
            onSuccess: Ok,
            onFailure: Problem
        );
    }

    [HttpGet("{id:guid}", Name = "GetTaskByIdV1")]
    public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await taskService.GetByIdAsync(id, UserId, cancellationToken);
        
        return result.Match(
            onSuccess: value => Ok(TaskResponse.Map(value)),
            onFailure: Problem
        );
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync(TaskRequest taskRequest, CancellationToken cancellationToken = default)
    {
        var result = await taskService.CreateAsync(taskDto: taskRequest.Map(UserId), cancellationToken);

        return result.Match(
            onSuccess: value => CreatedAtRoute(
                routeName: "GetTaskByIdV1",
                routeValues: new { id = value.Id },        
                TaskResponse.Map(value)
            ),
            onFailure: Problem
        );
    }

    [HttpPatch]
    public async Task<IActionResult> UpdateAsync(TaskRequest taskRequest, CancellationToken cancellationToken = default)
    {
        var result = await taskService.UpdateAsync(taskRequest.Map(UserId), cancellationToken);
        
        return result.Match(
            onSuccess: NoContent,
            onFailure: Problem
        );
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await taskService.DeleteAsync(id, UserId, cancellationToken);

        return result.Match(
            onSuccess: NoContent,
            onFailure: Problem
        );
    }
}
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
    [HttpGet("board")]
    public async Task<IActionResult> GetAsync(
        [FromQuery] int size, 
        CancellationToken cancellationToken = default)
    {
        var result = await taskService.GetBoardAsync(UserId, size, cancellationToken);
        
        return result.Match(
            onSuccess: Ok,
            onFailure: Problem
        );
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAsync(
        [FromQuery] GetTasksRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await taskService.GetAsync(
            userId: UserId,
            request.Skip,
            request.Size,
            request.State,
            cancellationToken);
        
        return result.Match(
            onSuccess: Ok,
            onFailure: Problem
        );
    }

    [HttpGet("{id:guid}", Name = nameof(GetByIdAsync))]
    public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await taskService.GetByIdAsync(
            taskId: id, 
            userId: UserId, 
            cancellationToken);
        
        return result.Match(
            onSuccess: Ok,
            onFailure: Problem
        );
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreateTaskRequest createTaskRequest, CancellationToken cancellationToken = default)
    {
        var result = await taskService.CreateAsync(
            taskDto: createTaskRequest.ToDto(userId: UserId), 
            cancellationToken: cancellationToken);

        return result.Match(
            onSuccess: value => CreatedAtRoute(
                routeName: nameof(GetByIdAsync),
                routeValues: new { id = value.Id },
                value
            ),
            onFailure: Problem
        );
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAsync(
        Guid id, 
        [FromBody] UpdateTaskDetailsRequest request, 
        CancellationToken cancellationToken = default)
    {
        var result = await taskService.UpdateAsync(
            taskId: id,
            taskDto: request.ToDto(userId: UserId), 
            cancellationToken: cancellationToken);
        
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

    [HttpPatch("{id:guid}/move")]
    public async Task<IActionResult> MoveAsync(
        Guid id, 
        [FromBody] TaskMoveRequest moveRequest, 
        CancellationToken cancellationToken = default)
    {
        var result = await taskService.MoveAsync(
            taskId: id, 
            userId: UserId, 
            newOrder: moveRequest.NewOrder,
            newState: moveRequest.NewState,
            cancellationToken);
        
        return result.Match(
            onSuccess: Ok,
            onFailure: Problem
        );
    }
}
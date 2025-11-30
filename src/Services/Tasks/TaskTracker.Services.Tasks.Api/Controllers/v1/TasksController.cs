using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskTracker.Services.Tasks.Api.Contracts.v1;
using TaskTracker.Services.Tasks.Api.Contracts.v1.Tasks;
using TaskTracker.Services.Tasks.ApplicationCore.Abstractions;
using TaskTracker.Services.Tasks.ApplicationCore.Common.Results;
using TaskTracker.Services.Tasks.ApplicationCore.DTOs.Tasks;

namespace TaskTracker.Services.Tasks.Api.Controllers.v1;

[Authorize]
[ApiController]
[ApiVersion(1)]
[Route("api/v{version:apiVersion}/tasks")]
public class TasksController(ITaskService taskService) : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var operationResult = await taskService.GetAllAsync(userId: UserId, cancellationToken);
        var tasks = operationResult.Value.Select(TaskResponse.Map);
        
        return Ok(tasks);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var operationResult = await taskService.GetByIdAsync(id, cancellationToken);
        var taskItem = operationResult.Value;

        if (taskItem is null || taskItem.UserId != UserId)
        {
            return NotFound();
        }
        var taskResponse = TaskResponse.Map(taskItem);
        
        return Ok(taskResponse);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync(TaskRequest taskRequest, CancellationToken cancellationToken = default)
    {
        var taskDto = new TaskDto(
            taskRequest.Id,
            taskRequest.Title,
            taskRequest.Description,
            taskRequest.State,
            taskRequest.SortOrder,
            UserId
        );

        var operationResult = await taskService.CreateAsync(taskDto, cancellationToken);
        var taskItem = operationResult.Value;
        var taskResponse = TaskResponse.Map(taskItem);

        return Ok(taskResponse);
    }

    [HttpPatch]
    public async Task<IActionResult> UpdateAsync(TaskRequest taskRequest, CancellationToken cancellationToken = default)
    {
        var taskDto = new TaskDto(
            taskRequest.Id,
            taskRequest.Title,
            taskRequest.Description,
            taskRequest.State,
            taskRequest.SortOrder,
            UserId
        );

        var operationResult = await taskService.UpdateAsync(taskDto, cancellationToken);

        if (operationResult is { Succeeded: false, Error.Code: ErrorCode.NotFoundError })
        {
            return Problem(
                detail: operationResult.Error.Description,
                statusCode: StatusCodes.Status404NotFound
            );
        }
        
        var taskItem = operationResult.Value;
        var taskResponse = TaskResponse.Map(taskItem);
        var apiResponse = new ApiResponseObject<TaskResponse>(
            $"Task \"{taskItem.Title}\" updated successfully.",  
            taskResponse);
        
        return Ok(apiResponse);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var operationResult = await taskService.GetByIdAsync(id, cancellationToken);
        var taskItem = operationResult.Value;
        
        if (taskItem is null || taskItem.UserId != UserId)
        {
            return NotFound();
        }

        await taskService.DeleteAsync(taskItem, cancellationToken);
        var apiResponse = new ApiResponse($"Task \"{taskItem.Title}\" deleted successfully.");
        return Ok(apiResponse);
    }
}
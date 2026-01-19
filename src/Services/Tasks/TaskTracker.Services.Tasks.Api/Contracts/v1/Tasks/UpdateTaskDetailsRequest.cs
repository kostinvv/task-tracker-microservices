using System.ComponentModel.DataAnnotations;
using TaskTracker.Services.Shared.Models;
using TaskTracker.Services.Tasks.ApplicationCore.DTOs.Tasks;

namespace TaskTracker.Services.Tasks.Api.Contracts.v1.Tasks;

public class UpdateTaskDetailsRequest
{
    [Required]
    [MaxLength(TaskItem.MaxTitleLength)]
    public required string Title { get; set; }

    [MaxLength(TaskItem.MaxDescriptionLength)]
    public string? Description { get; set; }
    
    public TaskDto ToDto(Guid userId) => new(Id: null, Title, Description, default, 0, userId);
}
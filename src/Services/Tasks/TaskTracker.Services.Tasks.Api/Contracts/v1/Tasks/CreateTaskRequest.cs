using System.ComponentModel.DataAnnotations;
using TaskTracker.Services.Tasks.ApplicationCore.DTOs.Tasks;
using TaskTracker.Services.Tasks.ApplicationCore.Models;

namespace TaskTracker.Services.Tasks.Api.Contracts.v1.Tasks;

public class CreateTaskRequest
{
    [Required]
    [MaxLength(TaskItem.MaxTitleLength)]
    public required string Title { get; set; }
    
    [MaxLength(TaskItem.MaxDescriptionLength)]
    public string? Description { get; set; }

    [Required]
    public required TaskState State { get; set; }

    [Required]
    public required int SortOrder { get; set; }
    
    public TaskDto ToDto(Guid userId) => new(Id: null, Title, Description, State, SortOrder, userId);
}
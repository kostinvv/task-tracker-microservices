using System.ComponentModel.DataAnnotations;
using TaskTracker.Services.Tasks.ApplicationCore.Models;

namespace TaskTracker.Services.Tasks.Api.Contracts.v1.Tasks;

public class GetTasksRequest
{
    [Required]
    [Range(1, 20)]
    public int Size { get; set; }

    [Required]
    public int AfterPosition { get; set; }
    
    [Required]
    public TaskState State { get; set; }
}
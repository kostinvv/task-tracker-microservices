using System.ComponentModel.DataAnnotations;

namespace TaskTracker.Services.Shared.Models;

public enum TaskState
{
    [Display(Name = "To Do")]
    ToDo = 0,
    
    [Display(Name = "In Progress")]
    InProgress = 1,
    
    [Display(Name = "Done")]
    Done = 2
}
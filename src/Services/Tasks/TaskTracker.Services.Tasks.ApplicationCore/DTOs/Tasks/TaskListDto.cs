using TaskTracker.Services.Tasks.ApplicationCore.Models;

namespace TaskTracker.Services.Tasks.ApplicationCore.DTOs.Tasks;

public class TaskListDto
{
    /// <summary>
    /// Идентификатор колонки с задачами.
    /// </summary>
    public TaskState Id { get; set; }

    /// <summary>
    /// Наименование колонки с задачами.
    /// </summary>
    public string Title { get; set; } = null!;

    /// <summary>
    /// Список задач.
    /// </summary>
    public IEnumerable<TaskDto> Tasks { get; set; } = [];
}
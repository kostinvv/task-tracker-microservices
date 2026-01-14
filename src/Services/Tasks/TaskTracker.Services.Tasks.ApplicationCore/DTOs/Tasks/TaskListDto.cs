using TaskTracker.Services.Tasks.ApplicationCore.Models;

namespace TaskTracker.Services.Tasks.ApplicationCore.DTOs.Tasks;

public class TaskListDto(TaskState id, string title, CursorList<TaskDto> cursorList)
{
    /// <summary>
    /// Идентификатор колонки с задачами.
    /// </summary>
    public TaskState Id { get; } = id;

    /// <summary>
    /// Наименование колонки с задачами.
    /// </summary>
    public string Title { get; } = title;

    /// <summary>
    /// Список задач.
    /// </summary>
    public CursorList<TaskDto> CursorList { get; } = cursorList;
}
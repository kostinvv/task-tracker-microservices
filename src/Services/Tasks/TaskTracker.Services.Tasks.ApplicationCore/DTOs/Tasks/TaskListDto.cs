using TaskTracker.Services.Shared.Models;

namespace TaskTracker.Services.Tasks.ApplicationCore.DTOs.Tasks;

public class TaskListDto(TaskState id, string title, PagedList<TaskDto> pagedList)
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
    public PagedList<TaskDto> PagedList { get; } = pagedList;
}
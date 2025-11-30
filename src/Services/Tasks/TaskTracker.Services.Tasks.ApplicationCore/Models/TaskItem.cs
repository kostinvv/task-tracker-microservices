using System.ComponentModel.DataAnnotations;

namespace TaskTracker.Services.Tasks.ApplicationCore.Models;

/// <summary> Пользовательская задача. </summary>
public class TaskItem
{
    public const int MaxTitleLength = 256;
    public const int MaxDescriptionLength = 1024;
    
    // EF
    private TaskItem() { }
    
    private TaskItem(
        string title, 
        string description, 
        TaskState taskState, 
        DateTime createdAt, 
        DateTime updatedAt, 
        int sortOrder,
        Guid userId)
    {
        Id = Guid.NewGuid();
        Title = title;
        Description = description;
        TaskState = taskState;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
        SortOrder = sortOrder;
        UserId = userId;
        User = null;
    }
    
    public Guid Id { get; private set; }
    
    [MaxLength(MaxTitleLength)]
    public string Title { get; private set; } = null!;
    
    [MaxLength(MaxDescriptionLength)]
    public string Description { get; private set; } = string.Empty;

    public TaskState TaskState { get; private set; }
    
    public DateTime CreatedAt { get; private set; }
    
    public DateTime UpdatedAt { get; private set; }
    
    public int SortOrder { get; private set; }
    
    public Guid UserId { get; private set; }
    
    public ApplicationUser? User { get; private set; }
    
    // TODO: У задачи может быть время выполнения.
    
    public static TaskItem Create(
        string title, 
        string description, 
        DateTime createdAt, 
        DateTime updatedAt, 
        Guid userId,
        int sortOrder = 0,
        TaskState taskState = TaskState.ToDo)
    {
        return new TaskItem(title, description, taskState, createdAt, updatedAt, sortOrder, userId);
    }

    public void Update(string title, string description, TaskState state, int sortOrder, DateTime updatedAt)
    {
        // TODO: Валидация сущности при обновлении.
        Title = title;
        Description = description;
        TaskState = state;
        SortOrder = sortOrder;
        UpdatedAt = updatedAt;
    }
}
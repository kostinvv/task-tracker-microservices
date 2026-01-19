using System.ComponentModel.DataAnnotations;

namespace TaskTracker.Services.Shared.Data.Entities;

/// <summary> Пользовательская задача. </summary>
public class TaskItem
{
    public const int MaxTitleLength = 256;
    public const int MaxDescriptionLength = 1024;
    
    private TaskState _taskState;
    private int _sortOrder;
    private string _title;
    private string? _description;
    
    private TaskItem(
        string title, 
        string? description, 
        TaskState taskState, 
        DateTime createdAt, 
        DateTime updatedAt, 
        int sortOrder,
        Guid userId)
    {
        Id = Guid.NewGuid();
        _title = title;
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
    public string Title
    {
        get => _title;
        set
        {
            _title = value;
            UpdatedAt  = DateTime.UtcNow;
        }
    }

    [MaxLength(MaxDescriptionLength)]
    public string? Description
    {
        get => _description;
        set
        {
            _description = value;
            UpdatedAt = DateTime.UtcNow;
        }
    }
    
    public TaskState TaskState
    {
        get => _taskState;
        set
        {
            if (_taskState == value)
            {
                return;
            }
            _taskState = value;
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    
    public int SortOrder
    {
        get => _sortOrder;
        set
        {
            if (_sortOrder == value) return;
            _sortOrder = value;
            UpdatedAt = DateTime.UtcNow;
        }
    }
    
    public Guid UserId { get; private set; }
    public ApplicationUser? User { get; private set; }
    
    public static TaskItem Create(
        string title, 
        string? description, 
        DateTime createdAt, 
        DateTime updatedAt, 
        Guid userId,
        int sortOrder = 0,
        TaskState taskState = TaskState.ToDo)
    {
        return new TaskItem(title, description, taskState, createdAt, updatedAt, sortOrder, userId);
    }
}
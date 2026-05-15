using TaskTracker.Enums;

namespace TaskTracker.Models;

public class TeamTask
{
    private static int _nextId = 1;
    public int Id { get; } = _nextId++;
    
    public required string Title { get; init; }
    public string? Description { get; init; }
    public string? AssignedTo { get; private set; }
    public DateTime? DueDate { get; init; }
    public WorkItemStatus Status { get; private set; }
    
    public bool IsOverdue =>
        DueDate.HasValue && DueDate.Value.Date < DateTime.Today;
    
    public string Label => AssignedTo ?? "Unassigned";
    
    public event EventHandler<TaskStatusChangedArgs>? StatusChanged;
    
    public void Assign(string user)
    {
        if (string.IsNullOrWhiteSpace(user))
            throw new ArgumentException("User not found");

        AssignedTo = user;
    }

    public void Transition(WorkItemStatus newStatus)
    {
        if (Status == newStatus)
            return;
        
        // Capture curr status and then update
        var oldStatus = Status;
        Status = newStatus;

        // Raise the event if subscribers exist
        StatusChanged?.Invoke(this, new TaskStatusChangedArgs
        {
            TaskId = Id,
            Title = Title,
            OldStatus = oldStatus,
            NewStatus = newStatus,
            AssignedTo = AssignedTo
        });
    }
    }
};
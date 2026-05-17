using TaskTracker.Enums;
using TaskTracker.Services;
using TaskTracker.Interfaces;

namespace TaskTracker.Models;

public class TeamTask : IAssignable, ITransitionable, ISchedulable
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

    public event Action<TaskStatusChangedArgs>? StatusChanged;

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

        var oldStatus = Status;
        Status = newStatus;

        StatusChanged?.Invoke(new TaskStatusChangedArgs
        {
            TaskId = Id,
            Title = Title,
            OldStatus = oldStatus,
            NewStatus = newStatus,
            AssignedTo = AssignedTo
        });
    }
}
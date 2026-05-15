using TaskTracker.Enums;

namespace TaskTracker.Services;

    public class TaskStatusChangedArgs : EventArgs
    {
        public int TaskId { get; init; }
        public string Title { get; init; } = string.Empty;

        public WorkItemStatus OldStatus { get; init; }
        public WorkItemStatus NewStatus { get; init; }

        public string? AssignedTo { get; init; }
    }
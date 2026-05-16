using TaskTracker.Enums;

namespace TaskTracker.Api.Models;

record CreateTaskRequest(
    string Title,
    string? Description,
    string? AssignedTo,
    DateTime? DueDate
);

record AssignRequest(string User);

record TransitionRequest(WorkItemStatus NewStatus);
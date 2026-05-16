using TaskTracker.Enums;
using TaskTracker.Models;
using TaskTracker.Services;
using TaskTracker.Api;

var task = new TeamTask
{
    Title = "Fix Output",
    Description = "Output value not showing"
};

// Audit logger logic
var auditLogger = new AuditLogger();
task.StatusChanged += auditLogger.OnStatusChanged;
task.StatusChanged += (sender, eventArgs) =>
{
    Console.WriteLine($"[Notify] \"{eventArgs.Title}\" is now {eventArgs.NewStatus}");
};
task.StatusChanged += (sender, eventArgs) =>
{
    if (eventArgs.NewStatus == WorkItemStatus.Done)
    {
        var name = string.IsNullOrWhiteSpace(eventArgs.AssignedTo)
            ? "someone"
            : eventArgs.AssignedTo;

        Console.WriteLine($"\"{eventArgs.Title}\" marked as Done by {name}");
    }
};

// Test
task.Assign("Alice");
task.Transition(WorkItemStatus.InProgress);
task.Transition(WorkItemStatus.InReview);
task.Transition(WorkItemStatus.Done);

// Builder logic
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<TaskStore>();

var app = builder.Build();
app.Run();
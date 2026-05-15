using TaskTracker.Enums;

namespace TaskTracker.Services;

public class AuditLogger
{
    public List<string> Log { get; } = new();

    public void OnStatusChanged(object? sender, TaskStatusChangedArgs e)
    {
        Log.Add(
            $@"[{DateTime.Now:yyyy-MM-dd HH:mm}] Task #{eventArgs.TaskId} ""{eventArgs.Title}"": {eventArgs.OldStatus} → {eventArgs.NewStatus}"
        );
    }
}
using TaskTracker.Enums;
using TaskTracker.Models;

namespace TaskTracker.Services;

public class AuditLogger
{
    public List<string> Log { get; } = new();

    public void LogStatusChange(TaskStatusChangedArgs statusEvent)
    {
        Log.Add(
            $@"[{DateTime.Now:yyyy-MM-dd HH:mm}] Task #{statusEvent.TaskId} ""{statusEvent.Title}"": {statusEvent.OldStatus} → {statusEvent.NewStatus}"
        );
    }
}
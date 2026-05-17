using TaskTracker.Interfaces;
using TaskTracker.Models;
using TaskTracker.Services;

namespace TaskTracker.Notifiers;

public class AuditNotifier : INotifier
{
    private readonly AuditLogger _auditLogger;

    public AuditNotifier(AuditLogger auditLogger)
    {
        _auditLogger = auditLogger;
    }

    public void Notify(TaskStatusChangedArgs args)
    {
        _auditLogger.LogStatusChange(args);
    }
}
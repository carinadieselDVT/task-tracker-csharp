namespace TaskTracker.Interfaces;

public interface ISchedulable
{
    DateTime DueDate { get; }
    bool IsOverdue { get; }
}
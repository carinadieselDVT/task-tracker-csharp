using TaskTracker.Enums;

public interface ITransitionable
{
    WorkItemStatus Status { get; }
    void Transition(WorkItemStatus newStatus);
}
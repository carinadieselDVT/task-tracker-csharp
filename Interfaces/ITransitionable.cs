using TaskTracker.Enums;

namespace TaskTracker.Interfaces;

public interface ITransitionable
{
    TaskStatus Status { get; }
    void Transition(TaskStatus newStatus);
}
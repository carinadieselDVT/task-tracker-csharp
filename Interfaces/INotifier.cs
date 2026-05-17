using TaskTracker.Events;

namespace TaskTracker.Interfaces
{
    public interface INotifier
    {
        void Notify(TaskStatusChangedArgs args);
    }
}
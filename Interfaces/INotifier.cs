using TaskTracker.Services;

namespace TaskTracker.Interfaces
{
    public interface INotifier
    {
        void Notify(TaskStatusChangedArgs args);
    }
}
using TaskTracker.Interfaces;
using TaskTracker.Models;
using TaskTracker.Services;

namespace TaskTracker.Notifiers;

public class ConsoleNotifier : INotifier
{
    public void Notify(TaskStatusChangedArgs args)
    {
        Console.WriteLine(
            $"Task #{args.TaskId} \"{args.Title}\" changed " +
            $"from {args.OldStatus} to {args.NewStatus}"
        );
    }
}
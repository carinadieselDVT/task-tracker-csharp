using TaskTracker.Interfaces;
using TaskTracker.Models;

namespace TaskTracker.Api;

public class InMemoryTaskRepository : ITaskRepository
{
    private readonly List<TeamTask> _tasks = new();

    public List<TeamTask> GetAll()
    {
        return _tasks;
    }

    public TeamTask? GetById(int id)
    {
        return _tasks.FirstOrDefault(t => t.Id == id);
    }

    public void Add(TeamTask task)
    {
        _tasks.Add(task);
    }
}
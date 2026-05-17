using TaskTracker.Models;

namespace TaskTracker.Interfaces;

public interface ITaskRepository
{
    List<TeamTask> GetAll();
    TeamTask? GetById(int id);
    void Add(TeamTask task);
}
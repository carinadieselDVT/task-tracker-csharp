using TaskTracker.Enums;
using TaskTracker.Models;
using TaskTracker.Api;
using TaskTracker.Interfaces;
using TaskTracker.Notifiers;
using TaskTracker.Services;

// Builder/Registration logic
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<ITaskRepository, InMemoryTaskRepository>();

builder.Services.AddSingleton<AuditLogger>();

builder.Services.AddSingleton<INotifier, AuditNotifier>();
builder.Services.AddSingleton<INotifier, ConsoleNotifier>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapPost("/api/tasks",
    (
        CreateTaskRequest request,
        ITaskRepository store,
        IEnumerable<INotifier> notifiers
    ) =>
    {
        if (string.IsNullOrWhiteSpace(request.Title))
        {
            return Results.BadRequest(new
            {
                message = "Title cannot be empty or whitespace"
            });
        }

        var task = new TeamTask
        {
            Title = request.Title,
            Description = request.Description,
            DueDate = request.DueDate
        };

        store.Add(task);

        foreach (var notifier in notifiers)
        {
            task.StatusChanged += notifier.Notify;
        }

        return Results.Created($"/api/tasks/{task.Id}", task);
    });

app.MapGet("/api/tasks",
    (ITaskRepository store) =>
{
    return Results.Ok(store.GetAll());
});

app.MapGet("/api/tasks/{id}",
    (int id, ITaskRepository store) =>
{
    var task = store.GetById(id);

    if (task is null)
    {
        return Results.NotFound(new
        {
            message = $"Task {id} was not found"
        });
    }

    return Results.Ok(task);
});

app.MapPatch("/api/tasks/{id}/assign",
    (
        int id,
        AssignRequest request,
        ITaskRepository store
    ) =>
    {
        var task = store.GetById(id);

        if (task is null)
        {
            return Results.NotFound(new
            {
                message = $"Task {id} was not found"
            });
        }

        task.Assign(request.User);

        return Results.NoContent();
    });

app.MapPatch("/api/tasks/{id}/status",
    (
        int id,
        TransitionRequest request,
        ITaskRepository store
    ) =>
    {
        var task = store.GetById(id);

        if (task is null)
        {
            return Results.NotFound(new
            {
                message = $"Task {id} was not found"
            });
        }

        if (task.Status == request.NewStatus)
        {
            return Results.BadRequest(new
            {
                message = $"Task already has status: '{request.NewStatus}'"
            });
        }

        task.Transition(request.NewStatus);

        return Results.NoContent();
    });

app.MapGet("/api/tasks/overdue",
    (ITaskRepository store) =>
{
    var overdueTasks = store
        .GetAll()
        .Where(task => task.IsOverdue)
        .ToList();

    return Results.Ok(overdueTasks);
});

app.Run();
using TaskTracker.Enums;
using TaskTracker.Models;
using TaskTracker.Services;
using TaskTracker.Api;
using TaskTracker.Api.Models;

// Builder/Registration logic
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<TaskStore>();
builder.Services.AddSingleton<AuditLogger>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapPost("/api/tasks",
    (
        CreateTaskRequest request,
        TaskStore store,
        AuditLogger auditLogger
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

        task.StatusChanged += auditLogger.OnStatusChanged;

        return Results.Created($"/api/tasks/{task.Id}", task);
    });
    
    app.MapGet("/api/tasks", (TaskStore store) =>
    {
        return Results.Ok(store.GetAll());
    });
    
    app.MapGet("/api/tasks/{id}", (int id, TaskStore store) =>
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
            TaskStore store
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
            TaskStore store
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
    
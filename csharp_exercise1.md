# Week 3 Weekend Exercise — Task Tracker Console App

**Duration:** This exercise is designed to be completed over the weekend (approx. 8–10 hours of actual work).  
**Submission:** Push your finished project to your personal GitHub repository and share the link before Monday's session.

---

## Overview

You will build a small task tracker as a C# console application. There is no starter code — you will create everything from scratch. The goal is not just to make it work, but to write it the way a professional would: with clean separation of responsibilities, safe handling of optional data, and a design that reacts to changes without things being tightly tangled together.

By the end you will have practised:

- Nullable reference types — marking what is optional and what is required
- Delegates and events — letting one part of the system react to another without them being directly connected
- REST API basics — exposing your tracker over HTTP using Minimal APIs
- SOLID principles — structuring your code so each piece has one clear job

---

## What You Are Building

A task tracker where users can:

- Create a task with a title, an optional description, and an optional due date
- Assign a task to a team member
- Move a task through statuses: `Backlog → InProgress → InReview → Done`
- See a live audit log of every status change
- Query tasks through a simple REST API

---

## Step 1 — Set Up Your Project

Before writing any domain code, get your project and repository ready.

### Criteria

- Create a new solution with `dotnet new` — use the `webapi` template since you will need it for Step 3
- Enable nullable reference types for the entire project by adding `<Nullable>enable</Nullable>` inside the `<PropertyGroup>` block of your `.csproj` file
- Initialise a Git repository in your project folder
- Make your first commit with just the empty project — the commit message should clearly describe what it contains
- Create a `README.md` file that describes what the project is. It does not need to be long — two or three sentences is fine. Commit it.

---

## Step 2 — Build the Domain Model

This is the core of your application. A domain model represents the real-world concept your app is about — in this case, a task.

### What a task looks like

A task has:

| Property | Type | Required? | Notes |
|---|---|---|---|
| `Id` | `int` | Yes | Auto-incremented, set on creation |
| `Title` | `string` | Yes | Cannot be null or empty |
| `Description` | `string?` | No | May or may not be provided |
| `AssignedTo` | `string?` | No | Null until someone is assigned |
| `DueDate` | `DateTime?` | No | Null if no deadline is set |
| `Status` | `TaskStatus` | Yes | Starts at `Backlog` |

A `TaskStatus` is an `enum` with four values: `Backlog`, `InProgress`, `InReview`, `Done`.

### Criteria

- Create a `TeamTask` class in its own file
- `Id` is a public read-only property generated automatically — each new task gets the next available number starting from 1
- `Title` uses the `required` keyword so the compiler enforces that it is always set when creating a task — it is typed as `string`, not `string?`
- `Description`, `AssignedTo`, and `DueDate` are typed with `?` because they are genuinely optional — the type annotation communicates this clearly to anyone reading the code
- `Status` is private-set so it can only be changed through a dedicated method, not by direct assignment from outside the class
- A method `Assign(string user)` is implemented. It sets `AssignedTo` to the provided value. If the provided string is null or whitespace it throws an `ArgumentException` with a clear message explaining what went wrong
- A method `Transition(TaskStatus newStatus)` is implemented. If the new status is the same as the current one, the method does nothing and returns. Otherwise it updates `Status`
- A read-only property `IsOverdue` returns `true` if `DueDate` has a value and that date is before today's date. If `DueDate` is null it returns `false` — it must never throw a `NullReferenceException`
- A read-only property `Label` returns `AssignedTo` if it has a value, or the string `"Unassigned"` if `AssignedTo` is null — use the null-coalescing operator `??` for this

### What good looks like

A `TeamTask` class that is clean, focused, and only knows about tasks. It does not print to the console, write to files, send emails, or talk to a database. Those are someone else's job.

---

## Step 3 — Add Events So the System Can React

Right now, when a task's status changes, nothing else knows about it. You will fix that by adding an event to `TeamTask`. An event lets other parts of the system subscribe and react — without `TeamTask` needing to know who is listening.

### What needs to happen when a status changes

1. An entry is written to an audit log recording what changed and when
2. A message is printed to the console so the user sees the change
3. If the new status is `Done`, a separate completion message is printed naming who completed it

`TeamTask` must not contain any of this logic. It simply announces that a change happened. Everything else reacts independently.

### Criteria

- Create a `TaskStatusChangedArgs` class that inherits from `EventArgs`. It must carry: `TaskId` (int), `Title` (string), `OldStatus` (TaskStatus), `NewStatus` (TaskStatus), and `AssignedTo` (string?) — `AssignedTo` is nullable because a task might not be assigned at the time of the change
- Add a public event to `TeamTask` typed as `EventHandler<TaskStatusChangedArgs>` and named `StatusChanged`
- Raise `StatusChanged` inside `Transition()` after the status has been updated. Use the null-conditional invoke pattern so that if no one is subscribed, nothing crashes
- The args object passed to the event must contain the correct values at the exact moment the transition happens — `OldStatus` must be captured before the update, `NewStatus` is the value being transitioned to
- Create an `AuditLogger` class in its own file. It has a public `List<string>` property called `Log`. It has a method that matches the `EventHandler<TaskStatusChangedArgs>` signature and can therefore be subscribed to the event directly. When called, it appends a string to `Log` in this format: `[YYYY-MM-DD HH:mm] Task #3 "Fix login bug": Backlog → InProgress`
- In `Program.cs`, after creating a task, subscribe the `AuditLogger`'s method to the task's `StatusChanged` event using `+=`
- In `Program.cs`, subscribe a second handler using a lambda expression. This handler prints a plain notification to the console each time a status changes, for example: `[Notify] "Fix login bug" is now InProgress`
- In `Program.cs`, subscribe a third handler using a lambda expression. This handler only does something when `NewStatus` is `Done`. It prints a completion message in this format: `✓ "Fix login bug" marked as Done by Alice` — if `AssignedTo` is null it prints `"someone"` instead of a name
- All three handlers must be subscribed using `+=` on the same event, and all three must fire every time `Transition()` is called with a new status

### What good looks like

You should be able to write code like this in `Program.cs` and see all three reactions fire:

```
var task = new TeamTask { Title = "Fix login bug" };
task.Assign("Alice");
// subscribe all three handlers here
task.Transition(TaskStatus.InProgress);
task.Transition(TaskStatus.InReview);
task.Transition(TaskStatus.Done);
```

---

## Step 4 — Expose It as a REST API

You will now wrap your domain model in a web API so it can be used over HTTP. Use ASP.NET Core Minimal APIs — this is a lightweight way to define endpoints without a lot of boilerplate.

### Endpoints to implement

| Method | Route | What it does | Success | Errors |
|---|---|---|---|---|
| `GET` | `/api/tasks` | Returns all tasks | `200 OK` | — |
| `GET` | `/api/tasks/{id}` | Returns one task by id | `200 OK` | `404` if not found |
| `POST` | `/api/tasks` | Creates a new task | `201 Created` + Location header | `400` if title is empty |
| `PATCH` | `/api/tasks/{id}/assign` | Assigns a task to someone | `204 No Content` | `404` if not found |
| `PATCH` | `/api/tasks/{id}/status` | Transitions the task status | `204 No Content` | `400` if same status · `404` if not found |
| `GET` | `/api/tasks/overdue` | Returns only overdue tasks | `200 OK` | — |

### Request body shapes

You will need these record types to receive data from the request body:

```csharp
record CreateTaskRequest(string Title, string? Description, string? AssignedTo, DateTime? DueDate);
record AssignRequest(string User);
record TransitionRequest(TaskStatus NewStatus);
```

### Criteria

- Create a `TaskStore` class in its own file. It holds a private `List<TeamTask>` and exposes three methods: one to get all tasks, one to find a task by id (returning `TeamTask?` — nullable, because it might not exist), and one to add a task
- Register `TaskStore` as a singleton in the DI container in `Program.cs` — this means every request shares the same in-memory list
- Register `AuditLogger` as a singleton in the DI container as well — the log must persist across requests
- When a task is created via `POST`, inject both `TaskStore` and `AuditLogger` into the endpoint handler. After adding the task to the store, subscribe the `AuditLogger`'s handler to the new task's `StatusChanged` event
- `GET /api/tasks` returns `200 OK` with the full list of tasks serialised as JSON
- `GET /api/tasks/{id}` returns `200 OK` with the matching task, or `404 Not Found` with a JSON error object containing a descriptive message if no task with that id exists
- `POST /api/tasks` checks that the `Title` field in the request body is not null or whitespace. If it is empty, return `400 Bad Request` with a JSON error object. If valid, create the task, add it to the store, and return `201 Created` — the response must include a `Location` header pointing to `/api/tasks/{newId}` and the body must contain the newly created task
- `PATCH /api/tasks/{id}/assign` looks up the task by id (returning `404` if not found), then calls `Assign()` with the user from the request body, and returns `204 No Content`
- `PATCH /api/tasks/{id}/status` looks up the task by id (returning `404` if not found). If the requested new status is the same as the current status, return `400 Bad Request` with a message explaining why. Otherwise call `Transition()` and return `204 No Content`
- `GET /api/tasks/overdue` returns `200 OK` with only the tasks where `IsOverdue` is `true`, filtered using LINQ
- No endpoint URL contains a verb — routes use nouns only (e.g. `/api/tasks`, not `/api/getTasks`)

---

## Step 5 — SOLID Design Review

Read through your own code and make the following changes. Each one addresses a real structural problem that is easy to fall into when building quickly.

### S — Single Responsibility

- `TeamTask` must only manage task state and raise events. If it does anything else, move that logic out
- `TaskStore` must only store and retrieve tasks. If it does any filtering, formatting, or notification, extract that into a separate class
- Each class lives in its own file — `TeamTask.cs`, `TaskStore.cs`, `AuditLogger.cs`, and so on

### O — Open/Closed

- Define an `INotifier` interface with a single method: `void Notify(TaskStatusChangedArgs args)`
- Create two classes that implement it: `ConsoleNotifier` (prints the notification message) and `AuditNotifier` (wraps `AuditLogger` and calls its log method)
- Replace the inline lambda subscribers in `Program.cs` with instances of these classes, subscribed via their `Notify` method
- Adding a future `EmailNotifier` or `SlackNotifier` must only require creating a new class — no existing class should need to be modified

### L — Liskov Substitution

- If you created a `ReadOnlyTask` that inherits from `TeamTask` but throws `NotSupportedException` in `Assign()` or `Transition()`, any code that accepts a `TeamTask` and calls those methods would break. This is an LSP violation. You do not need to create `ReadOnlyTask`, but confirm your class hierarchy does not have this problem. If it does, the fix is in the ISP step below.

### I — Interface Segregation

- Define three focused interfaces:
  - `IAssignable` — contains `void Assign(string user)`
  - `ITransitionable` — contains `void Transition(TaskStatus newStatus)` and the `Status` property
  - `ISchedulable` — contains the `DueDate` property and the `IsOverdue` property
- Make `TeamTask` implement all three
- Update the `PATCH /api/tasks/{id}/assign` endpoint so its handler works against `IAssignable` — it should not need to reference `TeamTask` directly

### D — Dependency Inversion

- Define an `ITaskRepository` interface with the same three methods currently on `TaskStore`: get all, get by id, and add
- Rename `TaskStore` to `InMemoryTaskRepository` and make it implement `ITaskRepository`
- Update the DI registration to: `builder.Services.AddSingleton<ITaskRepository, InMemoryTaskRepository>()`
- Update all endpoint handlers to depend on `ITaskRepository` through their parameter list — none of them should reference `InMemoryTaskRepository` directly
- The test: if someone wrote a `SqlTaskRepository` that also implemented `ITaskRepository`, the only line that would need to change is the registration in `Program.cs`. No endpoint handler would need to be touched.

---

## Submission Checklist

Before pushing to GitHub, go through this list:

- [ ] The project builds with no errors and no warnings
- [ ] `#nullable enable` (or the `.csproj` equivalent) is active
- [ ] `TeamTask`, `AuditLogger`, `InMemoryTaskRepository`, `ConsoleNotifier`, and `AuditNotifier` each live in their own file
- [ ] All five SOLID interfaces (`INotifier`, `IAssignable`, `ITransitionable`, `ISchedulable`, `ITaskRepository`) are defined
- [ ] All six API endpoints are implemented and return the correct status codes
- [ ] The audit log records every status transition and persists across API requests
- [ ] All three event subscribers fire when a task transitions
- [ ] The `README.md` describes the project and lists how to run it locally (`dotnet run`)
- [ ] Commit history is clean — multiple commits showing progress, not one large dump at the end

---

## A Note on Getting Stuck

Getting stuck is normal and expected. When it happens:

1. Re-read the criteria for the step you are on carefully
2. Check the Week 3 teaching materials for the relevant topic
3. Search the official Microsoft C# documentation — it is well written and beginner-friendly
4. If you are still stuck after 20–30 minutes, note exactly where you got stuck and what you tried. Bring that to Monday's session — it makes for a much better discussion than just saying something didn't work.

---

*Week 3 Weekend Exercise — due Monday before the session begins*

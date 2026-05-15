using TaskTracker.Enums;

namespace TaskTracker.Models;

public class TeamTask
{
    private static int _nextId = 1;
    public int Id { get; } = _nextId++;
    
    public required string Title { get; init; }
    public string? Description { get; init; }
    public string? AssignedTo { get; init; }
    public DateTime? DueDate { get; init; }
    public WorkItemStatus Status { get; private set; }
}



// A task has:
//
// Property	Type	Required?	Notes
// Id	int	Yes	Auto-incremented, set on creation
//     Title	string	Yes	Cannot be null or empty
// Description	string?	No	May or may not be provided
// AssignedTo	string?	No	Null until someone is assigned
//     DueDate	DateTime?	No	Null if no deadline is set
//     Status	TaskStatus	Yes	Starts at Backlog
//     A TaskStatus is an enum with four values: Backlog, InProgress, InReview, Done.
//
// Criteria
// Create a TeamTask class in its own file
// Id is a public read-only property generated automatically — each new task gets the next available number starting from 1
// Title uses the required keyword so the compiler enforces that it is always set when creating a task — it is typed as string, not string?
//     Description, AssignedTo, and DueDate are typed with ? because they are genuinely optional — the type annotation communicates this clearly to anyone reading the code
// Status is private-set so it can only be changed through a dedicated method, not by direct assignment from outside the class
//     A method Assign(string user) is implemented. It sets AssignedTo to the provided value. If the provided string is null or whitespace it throws an ArgumentException with a clear message explaining what went wrong
//     A method Transition(TaskStatus newStatus) is implemented. If the new status is the same as the current one, the method does nothing and returns. Otherwise it updates Status
// A read-only property IsOverdue returns true if DueDate has a value and that date is before today's date. If DueDate is null it returns false — it must never throw a NullReferenceException
//     A read-only property Label returns AssignedTo if it has a value, or the string "Unassigned" if AssignedTo is null — use the null-coalescing operator ?? for this
// What good looks like
// A TeamTask class that is clean, focused, and only knows about tasks. It does not print to the console, write to files, send emails, or talk to a database. Those are someone else's job.
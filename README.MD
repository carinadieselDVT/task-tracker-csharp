# Task Tracker

## Description

A team task tracker where you can create tasks, assign them to team members, move them through stages like Backlog, In Progress, In Review, and Done, and keep track of every status change with a live activity log. It also includes a basic REST API for fetching and managing tasks.

## How to Run

### Run the project

dotnet run

### Open Swagger UI

https://localhost:<port>/swagger

## How to Test

You can test the API using Swagger UI or Postman.

### 1. Create a task

POST /api/tasks

{
"title": "Build API",
"description": "Implement task tracker",
"dueDate": "2026-06-01"
}

### 2. Get all tasks

GET /api/tasks

### 3. Get task by ID

GET /api/tasks/{id}

### 4. Assign a task

PATCH /api/tasks/{id}/assign

{
"user": "John"
}

### 5. Change task status

PATCH /api/tasks/{id}/status

{
"newStatus": "InProgress"
}

### 6. Get overdue tasks

GET /api/tasks/overdue

## Swagger UI

https://localhost:<port>/swagger

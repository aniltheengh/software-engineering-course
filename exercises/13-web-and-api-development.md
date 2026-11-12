# Exercises 13 — Web and API development

Exercises 4–12 build one to-do API step by step.

### 1. Static page ★
A personal profile page in plain HTML and CSS, responsive with flexbox or grid, no framework.

### 2. Fetch and render ★★
With plain JavaScript, fetch JSON from a public API, render it as a list, and add a text box that filters the list as you type.

### 3. First endpoints ★
An ASP.NET Core minimal API with `GET /hello/{name}` and `GET /time`.

### 4. CRUD ★★
In-memory to-do API: `GET /todos`, `GET /todos/{id}`, `POST`, `PUT`, `DELETE`. Return 201 with a `Location` header on create, 404 for unknown ids, 204 on delete.

### 5. Validation ★★
Reject empty titles and past due dates with 400 and a `ProblemDetails` body. Add a global handler so unhandled exceptions return 500 without leaking a stack trace.

### 6. Persistence ★★
Replace the in-memory list with EF Core and SQLite, created by a migration.

### 7. Paging, filtering, sorting ★★★
Support `GET /todos?page=2&pageSize=10&done=false&sort=-dueDate`. Return the total count in the response and cap `pageSize`.

### 8. Middleware ★★
Write middleware that logs method, path, status code and duration for every request, and adds an `X-Correlation-Id` response header.

### 9. Authentication ★★★
Add register and login endpoints with hashed passwords, issue a JWT on login, and require it on all to-do endpoints. Each user sees only their own items.

### 10. Authorisation ★★
Add an `Admin` role that may delete any item.

### 11. API documentation ★
Expose an OpenAPI document and an interactive UI, with summaries and response types for every endpoint.

### 12. Integration tests ★★★
Using `WebApplicationFactory`, test the full create → read → update → delete flow, the 400 and 404 cases, and that one user cannot read another user's items.

### 13. Frontend ★★★
Build a Blazor or Razor Pages interface for the API with login, list, add, edit and complete.

### 14. Hardening ★★
Add rate limiting and a CORS policy that allows only your frontend's origin.

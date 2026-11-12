# Exercises 12 — Software design and architecture

Each exercise starts from a flawed design. Write the flawed version first so you can see the problem, then fix it.

### 1. Single responsibility ★★
A `Report` class loads data from a file, calculates totals, formats HTML and sends email. Split it so each class has one reason to change.

### 2. Open/closed ★★
A discount calculator uses `switch (customerType)`. Redesign it with the strategy pattern so a new customer type needs no edits to existing code.

### 3. Liskov substitution ★★
Make `Square` inherit from `Rectangle`, write a test that passes for `Rectangle` and fails for `Square`, then redesign.

### 4. Interface segregation ★
Split a fat `IMachine` (`Print`, `Scan`, `Fax`, `Staple`) so a simple printer does not implement methods it cannot support.

### 5. Dependency inversion ★★
A `NotificationService` creates its own `SmtpClient`. Invert the dependency and wire it with `Microsoft.Extensions.DependencyInjection`.

### 6. Factory ★★
Choose a parser from a file extension (`.csv`, `.json`, `.xml`) through a factory. Adding `.yaml` must touch one place only.

### 7. Observer ★★
A stock ticker publishes price changes to any number of subscribers that can join and leave at run time.

### 8. Decorator ★★★
Add caching and logging to an `IWeatherService` without modifying the original implementation, and make the two decorators stackable in either order.

### 9. Adapter ★★
A legacy class exposes `GetTempF(string zip)`. Your application expects `IWeatherService.GetTemperatureCelsius(Location)`. Write the adapter.

### 10. Command with undo ★★★
A text buffer supporting insert and delete commands with unlimited undo and redo.

### 11. Builder ★★
A fluent builder for an immutable `Report` or HTTP request object that validates required fields in `Build()`.

### 12. Layering ★★★
Restructure an earlier project into `Domain`, `Application` and `Infrastructure` projects. `Domain` must reference nothing; verify through the project references.

### 13. Decision record ★
Write an architecture decision record (context, decision, alternatives, consequences) for one real choice from exercise 12.

### 14. Code review ★★
Review your own Module 01 checkpoint project as if a colleague wrote it. List at least ten concrete issues, then fix the five most important.

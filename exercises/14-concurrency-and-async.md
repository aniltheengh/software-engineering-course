# Exercises 14 — Concurrency and async

### 1. Threads and tasks ★
Start ten workers that each print their managed thread id, first with `Thread`, then with `Task.Run`. Explain why the set of ids differs.

### 2. Async file I/O ★
Count the lines of every `.cs` file in a directory tree using `File.ReadLinesAsync` or `StreamReader.ReadLineAsync`.

### 3. Sequential vs concurrent ★
Simulate 20 calls of 500 ms each with `Task.Delay`. Await them one by one, then with `Task.WhenAll`.
Expected: about `10 s` versus about `0.5 s`

### 4. Throttling ★★
Run the 20 calls with at most 5 in flight at any moment, using `SemaphoreSlim`. Log the in-flight count to prove it.

### 5. Cancellation ★★
Make a long operation stop promptly when a `CancellationToken` is cancelled by Ctrl+C or after a three-second timeout.

### 6. Sync-over-async ★★★
In a small WinForms or WPF app, call an async method with `.Result` from a button handler and observe the deadlock. Explain it, then fix it properly.

### 7. Three fixes for a race ★★
Fix an unsynchronised shared counter with `lock`, with `Interlocked`, and by redesigning so no state is shared.

### 8. Lazy initialisation ★★
An expensive object must be created at most once even when 100 threads request it simultaneously. Solve it with `Lazy<T>`, then by hand with double-checked locking.

### 9. Channel pipeline ★★★
Three stages connected by bounded `Channel<T>`s: read lines from files, transform them, write results. Shut down cleanly when input ends and on cancellation.

### 10. Parallel speed-up ★★
Count the primes below 10,000,000 with 1, 2, 4 and 8 degrees of parallelism. Tabulate the times and the speed-up.
Expected count: `664,579`

### 11. Async streams ★★
Expose a paged data source as `IAsyncEnumerable<T>` and consume it with `await foreach`, stopping early after 25 items.

### 12. Progress ★
Report percentage complete from a background operation to the console with `IProgress<T>`.

### 13. Retry ★★★
Write `RetryAsync` with exponential backoff, random jitter, a maximum attempt count and cancellation. Test it against a fake operation that fails twice and then succeeds.

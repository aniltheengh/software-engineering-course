# Exercises 08 — Operating systems

### 1. Process list ★
Print the PID, name and memory use of the ten processes using the most memory (`Process.GetProcesses`).

### 2. Child process ★
Start `dotnet --version` as a child process, capture its standard output and print its exit code.

### 3. Pipes ★★
Start two child processes and connect the first one's standard output to the second one's standard input from your C# program.

### 4. Scheduling simulator ★★
Simulate FCFS, SJF and round-robin (quantum 2) and print the average waiting time for each.
Three jobs arriving at time 0 with bursts `5, 3, 8` → FCFS `4.33`, SJF `3.67`

### 5. Page replacement ★★★
Simulate FIFO, LRU and optimal replacement with 3 frames on the reference string
`7 0 1 2 0 3 0 4 2 3 0 3 2 1 2 0 1 7 0 1`.
Expected page faults: FIFO `15`, LRU `12`, optimal `9`

### 6. Race condition ★
Two threads each increment a shared `int` one million times without synchronisation. Run it ten times and record the results. Fix it.

### 7. Deadlock ★★
Write two threads that reliably deadlock on two locks. Fix it by always acquiring the locks in the same order.

### 8. Bounded buffer ★★★
A producer-consumer queue of capacity 10 built with `Monitor.Wait` and `Monitor.Pulse` (not `BlockingCollection`). Run three producers and two consumers and verify nothing is lost or duplicated.

### 9. Dining philosophers ★★★
Five philosophers, five forks, no deadlock and no starvation. Explain why your solution is safe.

### 10. Reader-writer cache ★★
A dictionary-backed cache protected by `ReaderWriterLockSlim`, allowing many concurrent readers and one writer.

### 11. Disk usage ★★
A `du`-style tool: print the ten largest subfolders of a directory, handling access-denied errors without stopping.

### 12. Memory-mapped file ★★★
Count the lines of a file of at least 1 GB using `MemoryMappedFile`, and again with `StreamReader`. Compare time and memory.

### 13. Tail ★★
Implement `tail -f`: print new lines as they are appended to a file.

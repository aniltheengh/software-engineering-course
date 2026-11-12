# 08 — Operating systems

**Duration:** 6 weeks · **Prerequisites:** 07

## Goals
- Explain what the OS does for a running program
- Understand processes, threads, virtual memory and file systems well enough to debug real problems

## Topics
1. Processes, system calls, user vs kernel mode
2. Scheduling
3. Virtual memory: paging, TLB, swapping
4. Threads, locks, condition variables, semaphores, deadlock
5. File systems: inodes, journaling, crash consistency
6. I/O and devices
7. Virtualisation and containers (overview)

## Resources
- **Primary:** [Operating Systems: Three Easy Pieces (free book)](https://pages.cs.wisc.edu/~remzi/OSTEP/)
- [CS:APP](https://csapp.cs.cmu.edu/) — chapters 8–10, 12
- [Linux command line basics — The Missing Semester](https://missing.csail.mit.edu/)
- [System.Diagnostics.Process documentation](https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.process)

## Practice
- OSTEP homework simulations for scheduling, paging and locks
- C# program that launches child processes and pipes data between them
- Produce a deadlock with two locks, then fix it by lock ordering
- Implement a bounded producer-consumer queue with `Monitor`, then with `SemaphoreSlim`

## Checkpoint project
A **simple shell** in C#: run programs, pipes, redirection, background jobs, built-in `cd` and `exit`.

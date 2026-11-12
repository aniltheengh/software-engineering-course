# 14 — Concurrency and async

**Duration:** 3 weeks · **Prerequisites:** 08

## Goals
- Use `async`/`await` correctly and explain what it compiles to
- Write thread-safe code and spot race conditions

## Topics
1. Concurrency vs parallelism
2. Threads and the thread pool
3. `Task`, `async`/`await`, cancellation, `ValueTask`
4. Common async pitfalls: blocking on async, `async void`, deadlocks
5. Synchronisation: `lock`, `Interlocked`, `SemaphoreSlim`, reader-writer locks
6. Concurrent collections and `Channel<T>`
7. Data parallelism: `Parallel.ForEachAsync`, PLINQ
8. Immutability as a concurrency strategy

## Resources
- **Primary:** [Asynchronous programming with async and await](https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/)
- [Threading in C# (Joseph Albahari, free online)](https://www.albahari.com/threading/)
- [Parallel programming in .NET](https://learn.microsoft.com/en-us/dotnet/standard/parallel-programming/)
- [System.Threading.Channels](https://learn.microsoft.com/en-us/dotnet/core/extensions/channels)
- Book: *Concurrency in C# Cookbook* (Cleary)

## Practice
- Download 50 URLs sequentially, then concurrently with a limit of 5; compare timings
- Break a counter with a race condition, then fix it three different ways
- Producer-consumer pipeline with `Channel<T>` and cancellation
- Parallelise a CPU-bound job and measure the speed-up against core count

## Checkpoint project
A **concurrent web crawler or batch file processor** with bounded parallelism, cancellation, retries and progress reporting.

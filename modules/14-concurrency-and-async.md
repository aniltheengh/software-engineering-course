# 14 — Concurrency and async

**Duration:** 3 weeks · **Prerequisites:** 08

**Exercises:** [13 exercises for this module](../exercises/14-concurrency-and-async.md)

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

## Code examples

All of the C# below is one runnable project in [examples/14-concurrency-and-async](../examples/14-concurrency-and-async). Run it with `dotnet run` from that folder, then change it and run it again.

### async/await: waiting without blocking a thread

```csharp
static class AsyncBasics
{
    static async Task<int> FetchAsync(int id)
    {
        await Task.Delay(300);   // stands in for a network or disk call
        return id * 10;
    }

    public static async Task Run()
    {
        var timer = Stopwatch.StartNew();
        int a = await FetchAsync(1);   // one after another
        int b = await FetchAsync(2);
        Console.WriteLine($"sequential: {a + b} in {timer.ElapsedMilliseconds} ms");   // about 600 ms

        timer.Restart();
        Task<int> first = FetchAsync(1);   // start both, then wait for both
        Task<int> second = FetchAsync(2);
        int[] results = await Task.WhenAll(first, second);
        Console.WriteLine($"concurrent: {results.Sum()} in {timer.ElapsedMilliseconds} ms");   // about 300 ms
    }
}
```

### Limiting how many operations run at once

```csharp
static class Throttling
{
    public static async Task Run()
    {
        using var limit = new SemaphoreSlim(3);
        int running = 0, peak = 0;

        async Task Work()
        {
            await limit.WaitAsync();
            try
            {
                int now = Interlocked.Increment(ref running);
                InterlockedMax(ref peak, now);
                await Task.Delay(100);
            }
            finally
            {
                Interlocked.Decrement(ref running);
                limit.Release();   // always release, even when the work throws
            }
        }

        await Task.WhenAll(Enumerable.Range(0, 12).Select(_ => Work()));
        Console.WriteLine($"12 jobs, at most {peak} at a time");   // 3
    }

    static void InterlockedMax(ref int target, int value)
    {
        int seen;
        while (value > (seen = Volatile.Read(ref target)))
            Interlocked.CompareExchange(ref target, value, seen);
    }
}
```

### Cancellation and timeouts

```csharp
static class Cancellation
{
    static async Task<int> CountAsync(CancellationToken token)
    {
        int steps = 0;
        while (true)
        {
            token.ThrowIfCancellationRequested();   // cooperative: the work checks the token
            await Task.Delay(50, token);
            steps++;
        }
    }

    public static async Task Run()
    {
        using var source = new CancellationTokenSource(TimeSpan.FromMilliseconds(220));   // cancel after a timeout
        try
        {
            await CountAsync(source.Token);
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("operation cancelled by timeout");
        }
    }
}
```

### Shared state: a race and two fixes

```csharp
static class SharedState
{
    public static void Run()
    {
        int racy = 0, atomic = 0, locked = 0;
        var gate = new object();

        Parallel.For(0, 1_000_000, _ =>
        {
            racy++;                               // not atomic: updates are lost
            Interlocked.Increment(ref atomic);    // a single atomic CPU operation
            lock (gate) { locked++; }             // mutual exclusion
        });

        Console.WriteLine($"racy={racy:N0}  atomic={atomic:N0}  locked={locked:N0}");
    }
}
```

### Channels: a producer and a consumer

```csharp
static class Pipelines
{
    public static async Task Run()
    {
        var channel = Channel.CreateBounded<int>(capacity: 5);   // a full channel makes the producer wait

        Task producer = Task.Run(async () =>
        {
            for (int i = 1; i <= 10; i++) await channel.Writer.WriteAsync(i);
            channel.Writer.Complete();   // tell the consumer there is no more
        });

        int sum = 0;
        await foreach (int item in channel.Reader.ReadAllAsync()) sum += item;

        await producer;
        Console.WriteLine($"consumer received a total of {sum}");   // 55
    }
}
```

### Parallel loops for CPU-bound work

```csharp
static class ParallelLoops
{
    static bool IsPrime(int n)
    {
        if (n < 2) return false;
        for (int i = 2; (long)i * i <= n; i++) if (n % i == 0) return false;
        return true;
    }

    public static void Run()
    {
        const int limit = 2_000_000;

        var timer = Stopwatch.StartNew();
        int sequential = Enumerable.Range(0, limit).Count(IsPrime);
        long sequentialMs = timer.ElapsedMilliseconds;

        timer.Restart();
        int parallel = Enumerable.Range(0, limit).AsParallel().Count(IsPrime);   // PLINQ
        long parallelMs = timer.ElapsedMilliseconds;

        Console.WriteLine($"{sequential} primes: {sequentialMs} ms on one core, {parallelMs} ms on {Environment.ProcessorCount} cores ({parallel})");
    }
}
```

### Async streams: produce values over time

```csharp
static class AsyncStreams
{
    static async IAsyncEnumerable<string> ReadPagesAsync()
    {
        for (int page = 1; page <= 3; page++)
        {
            await Task.Delay(50);          // fetch the next page
            yield return $"page {page}";
        }
    }

    public static async Task Run()
    {
        await foreach (string page in ReadPagesAsync())
            Console.WriteLine(page);       // each page is handled as soon as it arrives
    }
}
```

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

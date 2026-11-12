// Run with: dotnet run
using System.Diagnostics;
using System.Threading.Channels;

await AsyncBasics.Run();
await Throttling.Run();
await Cancellation.Run();
SharedState.Run();
await Pipelines.Run();
ParallelLoops.Run();
await AsyncStreams.Run();

// === async/await: waiting without blocking a thread ===
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

// === Limiting how many operations run at once ===
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

// === Cancellation and timeouts ===
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

// === Shared state: a race and two fixes ===
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

// === Channels: a producer and a consumer ===
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

// === Parallel loops for CPU-bound work ===
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

// === Async streams: produce values over time ===
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

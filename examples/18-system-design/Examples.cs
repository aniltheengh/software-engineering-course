// Run with: dotnet run
EstimationDemo.Run();
Base62Demo.Run();
RateLimiterDemo.Run();
CacheAsideDemo.Run();
LoadBalancerDemo.Run();

// === Back-of-the-envelope estimation ===
static class EstimationDemo
{
    public static void Run()
    {
        double dailyUsers = 2_000_000;
        double requestsPerUser = 20;
        double bytesPerRecord = 1_000;
        double writeShare = 0.1;

        double requestsPerDay = dailyUsers * requestsPerUser;
        double averagePerSecond = requestsPerDay / 86_400;
        double peakPerSecond = averagePerSecond * 3;          // rule of thumb: peak is 2-5x average
        double storagePerYearGb = requestsPerDay * writeShare * bytesPerRecord * 365 / 1e9;

        Console.WriteLine($"average load: {averagePerSecond:F0} requests/s");   // 463
        Console.WriteLine($"peak load:    {peakPerSecond:F0} requests/s");      // 1389
        Console.WriteLine($"new data:     {storagePerYearGb:F0} GB/year");      // 1460
    }
}

// === Short ids: base-62 encoding ===
static class Base62Demo
{
    const string Alphabet = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

    static string Encode(long number)
    {
        if (number == 0) return "0";
        var chars = new Stack<char>();
        while (number > 0)
        {
            chars.Push(Alphabet[(int)(number % 62)]);
            number /= 62;
        }
        return new string(chars.ToArray());
    }

    public static void Run()
    {
        Console.WriteLine(Encode(125));             // 21
        Console.WriteLine(Encode(1_000_000));       // 4c92
        Console.WriteLine(Math.Pow(62, 7).ToString("N0"));   // 3,521,614,606,208 ids fit in 7 characters
    }
}

// === Rate limiting: fixed window counter ===
class FixedWindowLimiter
{
    private readonly int _limit;
    private readonly TimeSpan _window;
    private readonly Dictionary<string, (DateTime WindowStart, int Count)> _clients = new();

    public FixedWindowLimiter(int limit, TimeSpan window) { _limit = limit; _window = window; }

    public bool Allow(string client, DateTime now)   // "now" is passed in so tests control time
    {
        if (!_clients.TryGetValue(client, out var state) || now - state.WindowStart >= _window)
            state = (now, 0);                        // start a new window

        if (state.Count >= _limit) return false;
        _clients[client] = (state.WindowStart, state.Count + 1);
        return true;
    }
}

static class RateLimiterDemo
{
    public static void Run()
    {
        var limiter = new FixedWindowLimiter(limit: 3, window: TimeSpan.FromSeconds(10));
        var start = new DateTime(2026, 1, 1, 12, 0, 0);

        for (int second = 0; second <= 11; second += 2)
            Console.Write($"t={second}s:{(limiter.Allow("client-1", start.AddSeconds(second)) ? "ok" : "429")}  ");
        Console.WriteLine();   // ok ok ok 429 429 ok   (the window resets at t=10s)
    }
}

// === Cache-aside with expiry ===
class Cache<TKey, TValue> where TKey : notnull
{
    private readonly Dictionary<TKey, (TValue Value, DateTime Expires)> _entries = new();
    private readonly TimeSpan _ttl;
    public int Hits { get; private set; }
    public int Misses { get; private set; }

    public Cache(TimeSpan ttl) => _ttl = ttl;

    public TValue GetOrLoad(TKey key, Func<TKey, TValue> load, DateTime now)
    {
        if (_entries.TryGetValue(key, out var entry) && entry.Expires > now) { Hits++; return entry.Value; }
        Misses++;
        TValue value = load(key);                     // the slow path: database or remote call
        _entries[key] = (value, now + _ttl);
        return value;
    }

    public void Invalidate(TKey key) => _entries.Remove(key);   // call this when the data changes
}

static class CacheAsideDemo
{
    public static void Run()
    {
        var cache = new Cache<int, string>(TimeSpan.FromSeconds(30));
        var now = new DateTime(2026, 1, 1);
        string LoadFromDatabase(int id) => $"product {id}";

        cache.GetOrLoad(7, LoadFromDatabase, now);                    // miss
        cache.GetOrLoad(7, LoadFromDatabase, now.AddSeconds(5));      // hit
        cache.GetOrLoad(7, LoadFromDatabase, now.AddSeconds(45));     // expired: miss
        Console.WriteLine($"hits={cache.Hits} misses={cache.Misses}");   // hits=1 misses=2
    }
}

// === Load balancing: round robin that skips unhealthy servers ===
class RoundRobinBalancer
{
    private readonly List<string> _servers;
    private readonly HashSet<string> _down = new();
    private int _next;

    public RoundRobinBalancer(IEnumerable<string> servers) => _servers = servers.ToList();
    public void MarkDown(string server) => _down.Add(server);

    public string Next()
    {
        for (int i = 0; i < _servers.Count; i++)
        {
            string candidate = _servers[_next];
            _next = (_next + 1) % _servers.Count;
            if (!_down.Contains(candidate)) return candidate;
        }
        throw new InvalidOperationException("no healthy servers");
    }
}

static class LoadBalancerDemo
{
    public static void Run()
    {
        var balancer = new RoundRobinBalancer(new[] { "app-1", "app-2", "app-3" });
        Console.WriteLine(string.Join(" ", Enumerable.Range(0, 5).Select(_ => balancer.Next())));   // app-1 app-2 app-3 app-1 app-2

        balancer.MarkDown("app-3");
        Console.WriteLine(string.Join(" ", Enumerable.Range(0, 4).Select(_ => balancer.Next())));   // app-1 app-2 app-1 app-2
    }
}

# 17 — Distributed systems

**Duration:** 5 weeks · **Prerequisites:** 08, 09, 10, 14

**Exercises:** [14 exercises for this module](../exercises/17-distributed-systems.md)

## Goals
- Explain why distributed systems fail in the ways they do
- Understand replication, partitioning and consensus at a working level

## Topics
1. Failure models; why networks and clocks cannot be trusted
2. Remote procedure calls and idempotency
3. Replication: leader-follower, multi-leader, leaderless
4. Partitioning and consistent hashing
5. Consistency models; CAP in practice
6. Consensus: Raft
7. Distributed transactions, sagas, outbox pattern
8. Messaging: queues, logs, delivery guarantees
9. Batch and stream processing (overview)

## Code examples

All of the C# below is one runnable project in [examples/17-distributed-systems](../examples/17-distributed-systems). Run it with `dotnet run` from that folder, then change it and run it again.

### Unreliable calls: timeouts and retries with backoff

```csharp
static class RetryDemo
{
    static int attempts;

    // Stands in for a remote call that fails twice before it works.
    static async Task<string> UnreliableCallAsync()
    {
        attempts++;
        await Task.Delay(20);
        if (attempts < 3) throw new TimeoutException($"attempt {attempts} timed out");
        return "response";
    }

    static async Task<T> RetryAsync<T>(Func<Task<T>> action, int maxAttempts)
    {
        var random = new Random();
        for (int attempt = 1; ; attempt++)
        {
            try
            {
                return await action();
            }
            catch (TimeoutException ex) when (attempt < maxAttempts)
            {
                int delayMs = (int)(50 * Math.Pow(2, attempt - 1)) + random.Next(0, 25);   // 50, 100, 200... plus jitter
                Console.WriteLine($"{ex.Message}; retrying in {delayMs} ms");
                await Task.Delay(delayMs);
            }
        }
    }

    public static async Task Run() => Console.WriteLine(await RetryAsync(UnreliableCallAsync, maxAttempts: 5));
}
```

### Idempotency: a retried request must not act twice

```csharp
static class IdempotencyDemo
{
    class PaymentService
    {
        private readonly Dictionary<string, decimal> _processed = new();   // request id -> result
        public decimal Balance { get; private set; } = 100m;

        public decimal Charge(string requestId, decimal amount)
        {
            if (_processed.TryGetValue(requestId, out decimal previous)) return previous;   // seen before: same answer
            Balance -= amount;
            _processed[requestId] = Balance;
            return Balance;
        }
    }

    public static void Run()
    {
        var service = new PaymentService();
        string requestId = Guid.NewGuid().ToString();   // the client chooses the id once

        service.Charge(requestId, 30m);
        service.Charge(requestId, 30m);   // the reply was lost, so the client retried
        Console.WriteLine(service.Balance);   // 70, not 40
    }
}
```

### Lamport clocks: ordering events without a shared clock

```csharp
static class LamportDemo
{
    class Node
    {
        public string Name { get; }
        public int Clock { get; private set; }
        public Node(string name) => Name = name;

        public int LocalEvent() => ++Clock;
        public int Send() => ++Clock;                                           // timestamp travels with the message
        public int Receive(int messageTime) => Clock = Math.Max(Clock, messageTime) + 1;
    }

    public static void Run()
    {
        var a = new Node("A");
        var b = new Node("B");

        a.LocalEvent();                 // A=1
        int stamp = a.Send();           // A=2
        b.LocalEvent();                 // B=1
        b.Receive(stamp);               // B = max(1, 2) + 1 = 3
        Console.WriteLine($"A={a.Clock} B={b.Clock}");   // A=2 B=3
        // If one event caused another, its timestamp is smaller. The reverse is not guaranteed.
    }
}
```

### Consistent hashing: adding a server moves few keys

```csharp
static class HashRingDemo
{
    static uint Hash(string text) => BitConverter.ToUInt32(SHA256.HashData(Encoding.UTF8.GetBytes(text)), 0);

    class Ring
    {
        private readonly SortedDictionary<uint, string> _points = new();

        public void AddServer(string server) => _points[Hash(server)] = server;

        public string ServerFor(string key)
        {
            uint h = Hash(key);
            foreach (var (point, server) in _points)
                if (point >= h) return server;    // first server clockwise from the key
            return _points.First().Value;         // wrap around the ring
        }
    }

    public static void Run()
    {
        var keys = Enumerable.Range(0, 10_000).Select(i => "key-" + i).ToList();

        var ring = new Ring();
        foreach (string s in new[] { "server-a", "server-b", "server-c" }) ring.AddServer(s);
        var before = keys.ToDictionary(k => k, ring.ServerFor);

        ring.AddServer("server-d");
        int moved = keys.Count(k => ring.ServerFor(k) != before[k]);
        Console.WriteLine($"ring: {moved * 100.0 / keys.Count:F0}% of keys moved");

        // Compare with "hash mod N", where changing N from 3 to 4 moves most keys.
        int movedModulo = keys.Count(k => Hash(k) % 3 != Hash(k) % 4);
        Console.WriteLine($"modulo: {movedModulo * 100.0 / keys.Count:F0}% of keys moved");   // about 75%
    }
}
```

### Quorums: overlapping majorities

```csharp
static class QuorumDemo
{
    public static void Run()
    {
        const int n = 3;   // replicas
        foreach (var (w, r) in new[] { (2, 2), (1, 1), (3, 1) })
        {
            bool consistent = w + r > n;   // every read set overlaps every write set
            Console.WriteLine($"N={n} W={w} R={r}: reads see latest write = {consistent}, " +
                              $"writes survive {n - w} failure(s), reads survive {n - r}");
        }
    }
}
```

### gRPC: a service contract in Protocol Buffers

From [`snippets/01-keyvalue.proto`](../examples/17-distributed-systems/snippets/01-keyvalue.proto).

```protobuf
// Add to an ASP.NET Core gRPC project (dotnet new grpc) under Protos/.
// The build generates C# base classes for the server and a typed client.
syntax = "proto3";

option csharp_namespace = "KeyValueStore";

service KeyValue {
  rpc Get (GetRequest) returns (GetReply);
  rpc Put (PutRequest) returns (PutReply);
}

message GetRequest {
  string key = 1;
}

message GetReply {
  bool found = 1;
  string value = 2;
}

message PutRequest {
  string key = 1;
  string value = 2;
  string request_id = 3;   // idempotency key: lets the server ignore a retried Put
}

message PutReply {
  int64 version = 1;
}
```

## Resources
- **Primary:** *Designing Data-Intensive Applications* (Kleppmann) — [book site](https://dataintensive.net/)
- [MIT 6.824 / 6.5840 Distributed Systems](https://pdos.csail.mit.edu/6.824/) — lectures and papers
- [The Raft consensus algorithm — paper and visualisation](https://raft.github.io/)
- [Distributed Systems lecture series — Martin Kleppmann (YouTube)](https://www.youtube.com/playlist?list=PLeKd45zvjcDFUEv_ohr_HdUFe97RItdiB)
- [gRPC for .NET](https://learn.microsoft.com/en-us/aspnet/core/grpc/)

## Practice
- Two services communicating over gRPC with retries and idempotency keys
- Consistent-hashing ring with virtual nodes
- Leader election among three local processes
- Read the Raft paper and write a one-page summary

## Checkpoint project
A **replicated key-value store** in C#: three nodes, leader-based replication, survives one node being killed. Full Raft is the stretch goal.

# 05 — Algorithms

**Duration:** 6 weeks · **Prerequisites:** 04

**Exercises:** [14 exercises for this module](../exercises/05-algorithms.md)

## Goals
- Recognise which algorithmic technique fits a problem
- Analyse running time and argue correctness

## Topics
1. Recursion and recurrence relations
2. Sorting: insertion, merge, quick, heap; stability; lower bound
3. Binary search and its variations
4. Two pointers and sliding window
5. Graph traversal: BFS, DFS, topological sort
6. Shortest paths: Dijkstra, Bellman-Ford; minimum spanning trees
7. Greedy algorithms
8. Dynamic programming
9. Backtracking
10. Overview of NP-completeness

## Code examples

All of the C# below is one runnable project in [examples/05-algorithms](../examples/05-algorithms). Run it with `dotnet run` from that folder, then change it and run it again.

### Recursion: a base case and a smaller problem

```csharp
static class RecursionDemo
{
    static long Factorial(int n) => n <= 1 ? 1 : n * Factorial(n - 1);

    static int SumDigits(int n) => n < 10 ? n : n % 10 + SumDigits(n / 10);

    public static void Run()
    {
        Console.WriteLine(Factorial(10));    // 3628800
        Console.WriteLine(SumDigits(9875));  // 29
    }
}
```

### Binary search: halve the range each step, O(log n)

```csharp
static class BinarySearchDemo
{
    static int Search(int[] sorted, int target)
    {
        int low = 0, high = sorted.Length - 1;
        while (low <= high)
        {
            int mid = low + (high - low) / 2;   // avoids overflow of (low + high)
            if (sorted[mid] == target) return mid;
            if (sorted[mid] < target) low = mid + 1;
            else high = mid - 1;
        }
        return -1;
    }

    public static void Run()
    {
        int[] data = { 2, 5, 8, 12, 16, 23, 38, 56, 72, 91 };
        Console.WriteLine(Search(data, 23));   // 5
        Console.WriteLine(Search(data, 7));    // -1
    }
}
```

### Merge sort: divide and conquer, O(n log n)

```csharp
static class MergeSortDemo
{
    static int[] Sort(int[] a)
    {
        if (a.Length <= 1) return a;
        int mid = a.Length / 2;
        int[] left = Sort(a[..mid]);
        int[] right = Sort(a[mid..]);

        var merged = new int[a.Length];
        int i = 0, j = 0, k = 0;
        while (i < left.Length && j < right.Length)
            merged[k++] = left[i] <= right[j] ? left[i++] : right[j++];   // <= keeps the sort stable
        while (i < left.Length) merged[k++] = left[i++];
        while (j < right.Length) merged[k++] = right[j++];
        return merged;
    }

    public static void Run() =>
        Console.WriteLine(string.Join(" ", Sort(new[] { 38, 27, 43, 3, 9, 82, 10 })));   // 3 9 10 27 38 43 82
}
```

### Graph traversal: BFS and DFS

```csharp
static class TraversalDemo
{
    static readonly Dictionary<string, string[]> Graph = new()
    {
        ["A"] = new[] { "B", "C" },
        ["B"] = new[] { "D" },
        ["C"] = new[] { "D", "E" },
        ["D"] = new[] { "F" },
        ["E"] = new[] { "F" },
        ["F"] = Array.Empty<string>()
    };

    // Breadth-first: nearest nodes first, using a queue. Gives shortest paths in edges.
    static List<string> Bfs(string start)
    {
        var order = new List<string>();
        var seen = new HashSet<string> { start };
        var queue = new Queue<string>();
        queue.Enqueue(start);
        while (queue.Count > 0)
        {
            string node = queue.Dequeue();
            order.Add(node);
            foreach (string next in Graph[node])
                if (seen.Add(next)) queue.Enqueue(next);   // Add returns false if already seen
        }
        return order;
    }

    // Depth-first: go as deep as possible, then back up. Recursion is the stack.
    static void Dfs(string node, HashSet<string> seen, List<string> order)
    {
        if (!seen.Add(node)) return;
        order.Add(node);
        foreach (string next in Graph[node]) Dfs(next, seen, order);
    }

    public static void Run()
    {
        Console.WriteLine("BFS: " + string.Join(" ", Bfs("A")));   // A B C D E F
        var order = new List<string>();
        Dfs("A", new HashSet<string>(), order);
        Console.WriteLine("DFS: " + string.Join(" ", order));      // A B D F C E
    }
}
```

### Greedy: take the best local choice (and when that fails)

```csharp
static class GreedyDemo
{
    static int GreedyCoins(int[] coinsDescending, int amount)
    {
        int count = 0;
        foreach (int coin in coinsDescending)
        {
            count += amount / coin;
            amount %= coin;
        }
        return count;
    }

    public static void Run()
    {
        Console.WriteLine(GreedyCoins(new[] { 25, 10, 5, 1 }, 63));   // 6  optimal for this coin system
        Console.WriteLine(GreedyCoins(new[] { 4, 3, 1 }, 6));         // 3  (4+1+1) but 3+3 needs only 2
        // Greedy is fast but only correct for some problems. When it fails, use dynamic programming.
    }
}
```

### Dynamic programming: store answers to sub-problems

```csharp
static class DynamicProgrammingDemo
{
    static long calls;

    static long FibSlow(int n) { calls++; return n < 2 ? n : FibSlow(n - 1) + FibSlow(n - 2); }

    static long FibMemo(int n, Dictionary<int, long> memo)
    {
        if (n < 2) return n;
        if (memo.TryGetValue(n, out long known)) return known;
        return memo[n] = FibMemo(n - 1, memo) + FibMemo(n - 2, memo);
    }

    static long FibTable(int n)
    {
        long previous = 0, current = 1;
        for (int i = 0; i < n; i++) (previous, current) = (current, previous + current);
        return previous;
    }

    public static void Run()
    {
        Console.WriteLine($"{FibSlow(30)} after {calls:N0} calls");   // 832040 after 2,692,537 calls
        Console.WriteLine(FibMemo(90, new Dictionary<int, long>()));  // 2880067194370816120
        Console.WriteLine(FibTable(90));                              // same, O(n) time and O(1) space
    }
}
```

### Backtracking: try, recurse, undo

```csharp
static class BacktrackingDemo
{
    static void Permute(List<char> remaining, List<char> current, List<string> results)
    {
        if (remaining.Count == 0) { results.Add(new string(current.ToArray())); return; }
        for (int i = 0; i < remaining.Count; i++)
        {
            char c = remaining[i];
            remaining.RemoveAt(i);            // choose
            current.Add(c);
            Permute(remaining, current, results);
            current.RemoveAt(current.Count - 1);   // undo the choice
            remaining.Insert(i, c);
        }
    }

    public static void Run()
    {
        var results = new List<string>();
        Permute(new List<char> { 'A', 'B', 'C' }, new List<char>(), results);
        Console.WriteLine(string.Join(" ", results));   // ABC ACB BAC BCA CAB CBA
    }
}
```

## Resources
- **Primary:** [MIT 6.006 Introduction to Algorithms (OCW)](https://ocw.mit.edu/courses/6-006-introduction-to-algorithms-spring-2020/)
- [Algorithms, 4th Edition — book site](https://algs4.cs.princeton.edu/) (Coursera *Algorithms, Part II*)
- [NeetCode roadmap](https://neetcode.io/roadmap) — ordered practice problems
- Book: *The Algorithm Design Manual* (Skiena)

## Practice
- Implement merge sort, quicksort, heap sort; compare timings with `Array.Sort`
- Implement BFS, DFS, Dijkstra and topological sort on your Module 04 graph
- Solve 60 problems from the NeetCode roadmap in C# (about 10 per week)

## Checkpoint project
A **route planner**: load a weighted graph from a file, compute shortest paths, print the route. Alternative: a pipe-network or dependency-order solver using topological sort.

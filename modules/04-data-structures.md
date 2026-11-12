# 04 — Data structures

**Duration:** 5 weeks · **Prerequisites:** 01–03

**Exercises:** [12 exercises for this module](../exercises/04-data-structures.md)

## Goals
- Implement the core data structures from scratch in C#
- Choose a structure based on the cost of the operations you need

## Topics
1. Big-O notation: time and space
2. Dynamic arrays and amortised cost
3. Linked lists (singly, doubly)
4. Stacks, queues, deques
5. Hash tables: hashing, collisions, load factor
6. Trees: binary search trees, traversals, balanced trees (AVL / red-black, conceptually)
7. Heaps and priority queues
8. Graphs: adjacency list vs matrix
9. Tries
10. How the .NET collections map to these (`List<T>`, `Dictionary`, `SortedSet`, `PriorityQueue`)

## Code examples

All of the C# below is one runnable project in [examples/04-data-structures](../examples/04-data-structures). Run it with `dotnet run` from that folder, then change it and run it again.

### Big-O in practice: the same question, two costs

```csharp
static class BigODemo
{
    public static void Run()
    {
        const int n = 200_000;
        var list = Enumerable.Range(0, n).ToList();
        var set = new HashSet<int>(list);

        var timer = Stopwatch.StartNew();
        for (int i = 0; i < 2_000; i++) list.Contains(n - 1);   // O(n): scans the whole list
        Console.WriteLine($"List.Contains:    {timer.ElapsedMilliseconds} ms");

        timer.Restart();
        for (int i = 0; i < 2_000; i++) set.Contains(n - 1);    // O(1): jumps to the bucket
        Console.WriteLine($"HashSet.Contains: {timer.ElapsedMilliseconds} ms");
    }
}
```

### Dynamic array: how List<T> grows

```csharp
class GrowableArray
{
    private int[] _items = new int[2];
    public int Count { get; private set; }
    public int Capacity => _items.Length;

    public void Add(int value)
    {
        if (Count == _items.Length)
        {
            var bigger = new int[_items.Length * 2];   // doubling keeps Add O(1) on average
            Array.Copy(_items, bigger, Count);
            _items = bigger;
        }
        _items[Count++] = value;
    }

    public int this[int index] => index < Count ? _items[index] : throw new IndexOutOfRangeException();
}

static class DynamicArrayDemo
{
    public static void Run()
    {
        var array = new GrowableArray();
        for (int i = 1; i <= 9; i++)
        {
            array.Add(i * 10);
            Console.Write($"{array.Count}/{array.Capacity}  ");   // 1/2 2/2 3/4 4/4 5/8 ... 9/16
        }
        Console.WriteLine($"\nelement 4 = {array[4]}");            // 50
    }
}
```

### Linked list: nodes that point to the next node

```csharp
class Node<T>
{
    public T Value;
    public Node<T>? Next;
    public Node(T value, Node<T>? next = null) { Value = value; Next = next; }
}

static class LinkedListDemo
{
    public static void Run()
    {
        Node<string>? head = null;
        foreach (string s in new[] { "c", "b", "a" })
            head = new Node<string>(s, head);        // insert at the front: O(1)

        for (var node = head; node != null; node = node.Next)   // walk the chain: O(n)
            Console.Write(node.Value + " -> ");
        Console.WriteLine("null");                   // a -> b -> c -> null
    }
}
```

### Stack (last in, first out) and queue (first in, first out)

```csharp
static class StackQueueDemo
{
    public static void Run()
    {
        var undo = new Stack<string>();
        undo.Push("type A"); undo.Push("type B"); undo.Push("delete");
        Console.WriteLine(undo.Pop());    // delete   (the most recent action)
        Console.WriteLine(undo.Peek());   // type B   (look without removing)

        var printJobs = new Queue<string>();
        printJobs.Enqueue("drawing-1"); printJobs.Enqueue("drawing-2");
        Console.WriteLine(printJobs.Dequeue());   // drawing-1   (the oldest job)
    }
}
```

### Hashing: from a key to a bucket

```csharp
static class HashingDemo
{
    public static void Run()
    {
        const int bucketCount = 8;
        var buckets = new List<string>[bucketCount];
        for (int i = 0; i < bucketCount; i++) buckets[i] = new List<string>();

        foreach (string key in new[] { "elbow", "tee", "valve", "flange", "reducer", "gasket" })
        {
            int index = (key.GetHashCode() & 0x7FFFFFFF) % bucketCount;   // non-negative, in range
            buckets[index].Add(key);                                       // two keys in one bucket = collision
        }

        for (int i = 0; i < bucketCount; i++)
            Console.WriteLine($"bucket {i}: {string.Join(", ", buckets[i])}");
        // String hash codes are randomised per run, so the layout changes each time.
    }
}
```

### Binary search tree: insert and in-order traversal

```csharp
class TreeNode
{
    public int Key;
    public TreeNode? Left, Right;
    public TreeNode(int key) => Key = key;
}

static class TreeDemo
{
    static TreeNode Insert(TreeNode? node, int key)
    {
        if (node == null) return new TreeNode(key);
        if (key < node.Key) node.Left = Insert(node.Left, key);
        else if (key > node.Key) node.Right = Insert(node.Right, key);
        return node;
    }

    static void InOrder(TreeNode? node, List<int> output)
    {
        if (node == null) return;
        InOrder(node.Left, output);    // everything smaller
        output.Add(node.Key);
        InOrder(node.Right, output);   // everything larger
    }

    public static void Run()
    {
        TreeNode? root = null;
        foreach (int key in new[] { 50, 30, 70, 20, 40, 60, 80 }) root = Insert(root, key);

        var sorted = new List<int>();
        InOrder(root, sorted);
        Console.WriteLine(string.Join(" ", sorted));   // 20 30 40 50 60 70 80
    }
}
```

### Heap: always get the smallest item first

```csharp
static class HeapDemo
{
    public static void Run()
    {
        var queue = new PriorityQueue<string, int>();   // a binary min-heap
        queue.Enqueue("repaint handrail", 5);
        queue.Enqueue("fix leaking flange", 1);
        queue.Enqueue("replace gasket", 3);

        while (queue.TryDequeue(out string? job, out int priority))
            Console.WriteLine($"{priority}: {job}");    // 1, 3, 5
    }
}
```

### Graph as an adjacency list

```csharp
static class GraphDemo
{
    public static void Run()
    {
        var graph = new Dictionary<string, List<string>>();

        void AddEdge(string a, string b)
        {
            if (!graph.ContainsKey(a)) graph[a] = new List<string>();
            if (!graph.ContainsKey(b)) graph[b] = new List<string>();
            graph[a].Add(b);
            graph[b].Add(a);   // undirected: store both directions
        }

        AddEdge("Tank", "Pump");
        AddEdge("Pump", "Filter");
        AddEdge("Pump", "Bypass");
        AddEdge("Filter", "Outlet");

        foreach (var (node, neighbours) in graph)
            Console.WriteLine($"{node}: {string.Join(", ", neighbours)}");
    }
}
```

## Resources
- **Primary:** [Algorithms, 4th Edition (Sedgewick & Wayne) — book site](https://algs4.cs.princeton.edu/) with the matching Coursera course *Algorithms, Part I* (Java; port to C#)
- [MIT 6.006 Introduction to Algorithms (OCW)](https://ocw.mit.edu/courses/6-006-introduction-to-algorithms-spring-2020/) — data-structure lectures
- [.NET collections and data structures](https://learn.microsoft.com/en-us/dotnet/standard/collections/)
- [VisuAlgo](https://visualgo.net/en) — animations

## Practice
Implement each with unit-style checks: dynamic array, linked list, stack, queue, hash map (separate chaining), binary search tree, min-heap, graph, trie. Then benchmark your hash map against `Dictionary`.

## Checkpoint project
A **collections library** (`MyCollections`) packaged as a class library with a console demo and a short README table of operation costs.

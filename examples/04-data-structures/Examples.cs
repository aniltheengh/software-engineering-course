// Run with: dotnet run
using System.Diagnostics;

BigODemo.Run();
DynamicArrayDemo.Run();
LinkedListDemo.Run();
StackQueueDemo.Run();
HashingDemo.Run();
TreeDemo.Run();
HeapDemo.Run();
GraphDemo.Run();

// === Big-O in practice: the same question, two costs ===
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

// === Dynamic array: how List<T> grows ===
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

// === Linked list: nodes that point to the next node ===
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

// === Stack (last in, first out) and queue (first in, first out) ===
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

// === Hashing: from a key to a bucket ===
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

// === Binary search tree: insert and in-order traversal ===
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

// === Heap: always get the smallest item first ===
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

// === Graph as an adjacency list ===
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

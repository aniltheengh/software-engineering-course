# Exercises 04 — Data structures

Implement everything from scratch. Do not use the .NET collection you are rebuilding. State the time complexity of each operation in a comment.

### 1. Dynamic array ★★
`MyList<T>` with `Add`, indexer, `RemoveAt`, `Count`, doubling its capacity when full. Starting at capacity 4, count the resizes for 1,000 `Add` calls.
Expected: `8` resizes.

### 2. Singly linked list ★★
`AddFirst`, `AddLast`, `Remove(value)`, `Reverse()` in place, and `IEnumerable<T>` so `foreach` works.

### 3. Stack and balanced brackets ★
Build an array-based stack and use it to check brackets.
`{[()]}` → `true`, `([)]` → `false`

### 4. Circular queue ★★
A fixed-capacity queue on an array with head and tail indices that wrap around. `Enqueue` on a full queue throws.

### 5. Queue from two stacks ★★
Implement a queue using only two of your stacks. Explain why `Dequeue` is O(1) amortised.

### 6. Hash map ★★★
Separate chaining with `Put`, `Get`, `Remove`, `ContainsKey`. Resize when the load factor passes 0.75.

### 7. Binary search tree ★★★
`Insert`, `Contains`, `Min`, `Max`, `Height`, in-order traversal, and `Delete` covering all three cases (leaf, one child, two children).

### 8. Min-heap ★★
`Push`, `Pop`, `Peek` on an array. Verify by pushing 1,000 random numbers and popping them all in sorted order.

### 9. Graph ★★
Adjacency-list graph with `AddEdge`, `Neighbours`, `Degree`, directed or undirected. Load it from a text file of `from to weight` lines.

### 10. Trie ★★
`Insert`, `Contains`, `StartsWith`, and `Autocomplete(prefix)` returning up to 10 words.

### 11. LRU cache ★★★
A cache of fixed capacity where `Get` and `Put` are both O(1) and the least recently used item is evicted when full. Build it from a dictionary and a doubly linked list.

### 12. Benchmark ★★
Time one million operations of `MyList<T>` against `List<T>`, and of your hash map against `Dictionary<TKey,TValue>`, using `Stopwatch`. Put the results in a table and explain the differences.

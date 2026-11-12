# 04 — Data structures

**Duration:** 5 weeks · **Prerequisites:** 01–03

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

## Resources
- **Primary:** [Algorithms, 4th Edition (Sedgewick & Wayne) — book site](https://algs4.cs.princeton.edu/) with the matching Coursera course *Algorithms, Part I* (Java; port to C#)
- [MIT 6.006 Introduction to Algorithms (OCW)](https://ocw.mit.edu/courses/6-006-introduction-to-algorithms-spring-2020/) — data-structure lectures
- [.NET collections and data structures](https://learn.microsoft.com/en-us/dotnet/standard/collections/)
- [VisuAlgo](https://visualgo.net/en) — animations

## Practice
Implement each with unit-style checks: dynamic array, linked list, stack, queue, hash map (separate chaining), binary search tree, min-heap, graph, trie. Then benchmark your hash map against `Dictionary`.

## Checkpoint project
A **collections library** (`MyCollections`) packaged as a class library with a console demo and a short README table of operation costs.

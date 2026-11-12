# 05 — Algorithms

**Duration:** 6 weeks · **Prerequisites:** 04

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

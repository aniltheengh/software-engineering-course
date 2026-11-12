# Exercises 05 — Algorithms

For each exercise, state the time and space complexity of your solution.

### 1. Binary search ★
Iterative binary search, then a version that returns the first occurrence among duplicates.
`[1, 2, 2, 2, 3]`, target `2` → index `1`

### 2. Three sorts ★★
Implement insertion sort, merge sort and quicksort. Check each against `Array.Sort` on 1,000 random arrays, then time all four on 1,000,000 elements.

### 3. Count inversions ★★★
Count pairs `i < j` with `a[i] > a[j]` in O(n log n) by modifying merge sort.
`[2, 4, 1, 3, 5]` → `3`

### 4. Pair sums ★
In a sorted array, find all pairs that add up to a target using two pointers.
`[1, 3, 4, 6, 9]`, target `10` → `(1, 9)` and `(4, 6)`

### 5. Longest substring without repeats ★★
Sliding window.
`abcabcbb` → `3`

### 6. Maze shortest path ★★
Read a grid of `.` (open), `#` (wall), `S` and `E`. Print the minimum number of steps from S to E using BFS, or `no path`.

### 7. Count islands ★★
In a grid of 0s and 1s, count groups of 1s connected horizontally or vertically, using DFS.

### 8. Task ordering ★★
Given tasks and "A must come before B" pairs, print a valid order with topological sort. Report clearly when there is a cycle.

### 9. Dijkstra ★★★
Shortest distances from a source in a weighted graph using `PriorityQueue<TElement,TPriority>`. Also print the path to a chosen target.

### 10. Interval scheduling ★★
Choose the maximum number of non-overlapping intervals (an interval may start when another ends).
`(1,3) (2,4) (3,5) (0,6) (5,7)` → `3`

### 11. Coin change ★★
Minimum number of coins to make an amount, by dynamic programming.
Coins `1, 5, 10, 25`, amount `63` → `6`

### 12. Longest common subsequence ★★★
`AGGTAB` and `GXTXAYB` → length `4` (`GTAB`). Print the subsequence, not only its length.

### 13. 0/1 knapsack ★★★
Weights `1, 3, 4, 5`, values `1, 4, 5, 7`, capacity `7` → best value `9`.

### 14. N-Queens ★★★
Count the ways to place N queens on an N×N board by backtracking.
`N = 8` → `92`

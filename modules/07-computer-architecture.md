# 07 — Computer architecture

**Duration:** 5 weeks · **Prerequisites:** 01–05

## Goals
- Explain how a program becomes instructions executed by hardware
- Reason about memory layout, caches and performance

## Topics
1. Binary, two's complement, floating point (IEEE 754)
2. Logic gates, adders, memory elements
3. CPU: instruction set, fetch-decode-execute, pipelining
4. Assembly basics and the call stack
5. Memory hierarchy and cache behaviour
6. Stack vs heap; how the .NET runtime, JIT and garbage collector fit in
7. Linking and loading

## Resources
- **Primary:** [Nand2Tetris](https://www.nand2tetris.org/) — Part I (build a computer from NAND gates)
- [Computer Systems: A Programmer's Perspective (CS:APP)](https://csapp.cs.cmu.edu/) — chapters 1–3, 6
- [.NET garbage collection fundamentals](https://learn.microsoft.com/en-us/dotnet/standard/garbage-collection/fundamentals)
- [What Every Programmer Should Know About Memory (Drepper)](https://people.freebsd.org/~lstewart/articles/cpumemory.pdf)

## Practice
- Nand2Tetris projects 1–6
- Write a C# program showing floating-point rounding error and fix it with `decimal`
- Measure row-major vs column-major traversal of a large 2D array and explain the gap
- Use `Span<T>` and `stackalloc` to remove allocations from a hot loop

## Checkpoint project
An **assembler or CPU emulator** for the Nand2Tetris Hack machine, written in C#.

# 01 — Programming fundamentals in C#

**Duration:** 6 weeks · **Prerequisites:** none

**Exercises:** [12 exercises for this module](../exercises/01-programming-fundamentals.md)

## Goals
- Write, run and debug console programs in C# without copying from a tutorial
- Break a problem into functions and choose suitable built-in types and collections

## Topics
1. The .NET toolchain: `dotnet new`, `dotnet run`, project files
2. Types, variables, operators, strings, type conversion
3. Control flow: `if`, `switch`, loops
4. Methods: parameters, return values, overloads, scope
5. Arrays, `List<T>`, `Dictionary<TKey,TValue>`
6. Value types vs reference types, `null` and nullable reference types
7. Exceptions and basic file I/O
8. LINQ basics: `Where`, `Select`, `OrderBy`, `GroupBy`

## Resources
- **Primary:** [Microsoft Learn — Get started with C#](https://learn.microsoft.com/en-us/training/paths/get-started-c-sharp-part-1/) (continue through the following parts)
- [C# fundamentals documentation](https://learn.microsoft.com/en-us/dotnet/csharp/tour-of-csharp/)
- [Exercism C# track](https://exercism.org/tracks/csharp) — graded practice exercises
- [CS50x](https://cs50.harvard.edu/x/) — weeks 0–5 for the underlying ideas (taught in C)

## Practice
- Complete 30 Exercism C# exercises
- Unit converter (length, pressure, flow rate) with input validation
- Read a CSV file, compute min/max/average per column, write a summary file
- Word-frequency counter using `Dictionary` and then again using LINQ

## Checkpoint project
A command-line **expense or inventory tracker**: add, list, filter and delete records, saved to a JSON file. Must handle bad input without crashing.

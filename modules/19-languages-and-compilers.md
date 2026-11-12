# 19 — Programming languages and compilers (optional)

**Duration:** 4 weeks · **Prerequisites:** 04, 05, 07

**Exercises:** [14 exercises for this module](../exercises/19-languages-and-compilers.md)

## Goals
- Understand how source code is parsed, analysed and executed
- Be able to build small domain-specific languages and code-analysis tools

## Topics
1. Lexing and tokens
2. Grammars and recursive-descent parsing
3. Abstract syntax trees and the visitor pattern
4. Semantic analysis: scopes, types
5. Tree-walking interpreters
6. Bytecode and virtual machines
7. The .NET pipeline: Roslyn, IL, JIT
8. Other paradigms: a taste of functional programming (F#)

## Resources
- **Primary:** [Crafting Interpreters (free online)](https://craftinginterpreters.com/) — Part II in Java; port to C#
- [The .NET Compiler Platform SDK (Roslyn)](https://learn.microsoft.com/en-us/dotnet/csharp/roslyn-sdk/)
- [Nand2Tetris](https://www.nand2tetris.org/) — Part II (compiler and OS)
- [F# documentation](https://learn.microsoft.com/en-us/dotnet/fsharp/)

## Practice
- Arithmetic expression evaluator with correct precedence
- Port the Lox tree-walking interpreter to C#
- Write a Roslyn analyzer that flags one code smell
- Solve five Module 05 problems in F#

## Checkpoint project
An **interpreter for a small language** of your own design, or a DSL for a real task such as describing parametric components or report templates.

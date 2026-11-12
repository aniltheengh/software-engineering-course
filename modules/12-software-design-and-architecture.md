# 12 — Software design and architecture

**Duration:** 4 weeks · **Prerequisites:** 03, 11

**Exercises:** [14 exercises for this module](../exercises/12-software-design-and-architecture.md)

## Goals
- Structure a medium-sized codebase so it stays easy to change
- Know the common patterns and, more importantly, when not to use them

## Topics
1. Coupling, cohesion, information hiding
2. SOLID principles
3. Design patterns: strategy, factory, observer, decorator, adapter, command, builder
4. Dependency injection and inversion of control
5. Layered, hexagonal and clean architecture
6. Domain modelling basics
7. API design and versioning
8. Requirements, UML sketches, architecture decision records

## Resources
- **Primary:** [Refactoring Guru — Design Patterns](https://refactoring.guru/design-patterns) (C# examples)
- [.NET application architecture guides](https://learn.microsoft.com/en-us/dotnet/architecture/)
- [Dependency injection in .NET](https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection)
- Books: *A Philosophy of Software Design* (Ousterhout), *Head First Design Patterns*, *Clean Architecture* (Martin)

## Practice
- Implement seven patterns above, each solving a concrete problem rather than a toy
- Find a SOLID violation in your old code for each principle and fix it
- Rewire a project to use `Microsoft.Extensions.DependencyInjection`
- Write an architecture decision record for a real choice you made

## Checkpoint project
A **plugin-based application**: a host that discovers and loads plugins through an interface (for example a file-conversion or report-generation tool), with a design document.

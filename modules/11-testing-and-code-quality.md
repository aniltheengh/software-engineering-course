# 11 — Testing and code quality

**Duration:** 3 weeks · **Prerequisites:** 03

## Goals
- Write code that is testable, and tests that are worth keeping
- Refactor safely

## Topics
1. Unit, integration and end-to-end tests; the test pyramid
2. xUnit: facts, theories, fixtures
3. Test doubles: stubs, mocks, fakes
4. Test-driven development
5. Code coverage and its limits
6. Refactoring and code smells
7. Static analysis, formatting, code review
8. Logging and diagnostics

## Resources
- **Primary:** [Unit testing C# with xUnit and dotnet test](https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-csharp-with-xunit)
- [Unit testing best practices for .NET](https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-best-practices)
- [xUnit.net documentation](https://xunit.net/)
- [Refactoring Guru — Refactoring](https://refactoring.guru/refactoring)
- Books: *Refactoring* (Fowler), *Unit Testing Principles, Practices, and Patterns* (Khorikov)

## Practice
- Add xUnit tests to the Module 04 collections library, aiming for meaningful coverage
- Do the Bowling Game and String Calculator katas with strict TDD
- Take the Gilded Rose kata: add tests first, then refactor
- Enable .NET analyzers and `dotnet format`; fix every warning

## Checkpoint project
Take one earlier project to **production quality**: test suite, analyzers clean, structured logging, and a short document of the refactorings you made.

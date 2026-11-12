# 03 — Object-oriented programming

**Duration:** 4 weeks · **Prerequisites:** 01–02

## Goals
- Model a problem domain with classes, interfaces and records
- Explain when to use inheritance and when composition is better

## Topics
1. Classes, objects, constructors, properties, access modifiers
2. Encapsulation and invariants
3. Inheritance, `virtual`/`override`, abstract classes
4. Interfaces and polymorphism
5. Composition over inheritance
6. Generics and constraints
7. Records, structs, enums, immutability
8. Delegates, lambdas, events
9. `IDisposable` and `using`

## Resources
- **Primary:** [Object-oriented programming in C#](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/tutorials/oop)
- [C# fundamentals: types, classes, interfaces, generics](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/types/)
- [Delegates and events](https://learn.microsoft.com/en-us/dotnet/csharp/delegates-overview)
- Book: *Head First C#* (Stellman & Greene) or *C# in Depth* (Jon Skeet) for a deeper pass

## Practice
- Shape hierarchy with area/perimeter, first by inheritance, then by interface
- Bank account model that makes invalid states impossible
- Generic `Result<T>` type and a generic repository interface
- Event-driven temperature monitor using delegates and events

## Checkpoint project
A **library management** or **parts catalogue** system: at least five classes, two interfaces, one generic type, persistence behind an interface so the storage can be swapped.

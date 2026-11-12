# Software Engineering Course (C#)

A self-study curriculum covering the breadth of a computer science degree plus the engineering practice needed to ship real software. Every module has a topic list, runnable C# code examples, curated free (or standard-textbook) resources, a graded exercise set, and a checkpoint project.

- **Language:** C# / .NET (concepts are language-independent; a few systems modules read C)
- **Pace:** 8–10 hours per week
- **Length:** about 84 weeks core, 88 with the optional module
- **Code examples:** [examples/](examples/README.md) — a runnable C# project per module, also shown inside each module page
- **Exercises:** [exercises/](exercises/README.md) — one graded set per module
- **Track your progress:** [PROGRESS.md](PROGRESS.md)

## How to use this course

1. Work through the modules in order; later phases assume the earlier ones.
2. In each module, do the **primary resource** first. The others are for a second explanation or extra depth.
3. Read the module's **code examples**, run them from [examples/](examples/README.md), and change them until you can predict the output.
4. Do the module's exercise set in [exercises/](exercises/README.md) — over 250 graded exercises, many with expected results to check against. Reading without writing code does not stick.
5. Build the checkpoint project and push it to GitHub before moving on.
6. Tick the module off in `PROGRESS.md`.

## Roadmap

### Phase 1 — Foundations (27 weeks)

| # | Module | Weeks |
|---|--------|-------|
| 01 | [Programming fundamentals in C#](modules/01-programming-fundamentals.md) | 6 |
| 02 | [Developer tools: shell, Git, debugging](modules/02-developer-tools.md) | 2 |
| 03 | [Object-oriented programming](modules/03-object-oriented-programming.md) | 4 |
| 04 | [Data structures](modules/04-data-structures.md) | 5 |
| 05 | [Algorithms](modules/05-algorithms.md) | 6 |
| 06 | [Discrete mathematics](modules/06-discrete-mathematics.md) | 4 |

### Phase 2 — Systems (20 weeks)

| # | Module | Weeks |
|---|--------|-------|
| 07 | [Computer architecture](modules/07-computer-architecture.md) | 5 |
| 08 | [Operating systems](modules/08-operating-systems.md) | 6 |
| 09 | [Computer networking](modules/09-networking.md) | 4 |
| 10 | [Databases](modules/10-databases.md) | 5 |

### Phase 3 — Engineering practice (22 weeks)

| # | Module | Weeks |
|---|--------|-------|
| 11 | [Testing and code quality](modules/11-testing-and-code-quality.md) | 3 |
| 12 | [Software design and architecture](modules/12-software-design-and-architecture.md) | 4 |
| 13 | [Web and API development](modules/13-web-and-api-development.md) | 5 |
| 14 | [Concurrency and async](modules/14-concurrency-and-async.md) | 3 |
| 15 | [Security](modules/15-security.md) | 3 |
| 16 | [DevOps: CI/CD, containers, cloud](modules/16-devops.md) | 4 |

### Phase 4 — Advanced and capstone (15–19 weeks)

| # | Module | Weeks |
|---|--------|-------|
| 17 | [Distributed systems](modules/17-distributed-systems.md) | 5 |
| 18 | [System design](modules/18-system-design.md) | 4 |
| 19 | [Programming languages and compilers (optional)](modules/19-languages-and-compilers.md) | 4 |
| 20 | [Capstone project](modules/20-capstone.md) | 6 |

## General references

- [Teach Yourself Computer Science](https://teachyourselfcs.com) — the reasoning behind this subject list
- [OSSU Computer Science](https://github.com/ossu/computer-science) — a full free CS degree path
- [roadmap.sh](https://roadmap.sh) — role-based roadmaps (backend, DevOps, ASP.NET Core)
- [C# documentation](https://learn.microsoft.com/en-us/dotnet/csharp/) — the official language reference

## Setup

- Install the [.NET SDK](https://dotnet.microsoft.com/download) (current LTS)
- Editor: Visual Studio Community (Windows) or VS Code with the C# Dev Kit
- Git, and a GitHub account
- Create one repository per checkpoint project, or keep them all under a `projects/` folder in a fork of this repo

# 16 — DevOps: CI/CD, containers, cloud

**Duration:** 4 weeks · **Prerequisites:** 11, 13

**Exercises:** [14 exercises for this module](../exercises/16-devops.md)

## Goals
- Take an application from a commit to a running deployment automatically
- Observe it once it is running

## Topics
1. Linux server basics
2. Continuous integration: build, test, lint on every push
3. Docker: images, containers, volumes, networks, Compose
4. Continuous delivery and deployment strategies
5. Cloud fundamentals: compute, storage, managed databases
6. Infrastructure as code (overview)
7. Observability: logs, metrics, traces
8. Kubernetes (overview only)

## Resources
- **Primary:** [GitHub Actions documentation](https://docs.github.com/en/actions) and [Docker — Get started](https://docs.docker.com/get-started/)
- [Containerize a .NET app](https://learn.microsoft.com/en-us/dotnet/core/docker/build-container)
- [The Twelve-Factor App](https://12factor.net/)
- [Google SRE Book (free online)](https://sre.google/sre-book/table-of-contents/)
- [roadmap.sh — DevOps](https://roadmap.sh/devops)
- [.NET observability with OpenTelemetry](https://learn.microsoft.com/en-us/dotnet/core/diagnostics/observability-with-otel)

## Practice
- GitHub Actions workflow that builds and tests on every pull request
- Multi-stage Dockerfile for your web app; run app + database with Docker Compose
- Deploy to a free or low-cost cloud tier
- Add health checks, structured logs and one metric

## Checkpoint project
Your Module 13 application with a **complete pipeline**: push to `main` runs tests, builds an image and deploys it, with a status badge in the README.

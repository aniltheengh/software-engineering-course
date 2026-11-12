# 18 — System design

**Duration:** 4 weeks · **Prerequisites:** 10, 13, 16, 17

**Exercises:** [14 exercises for this module](../exercises/18-system-design.md)

## Goals
- Turn vague requirements into a defensible architecture
- Estimate capacity and explain trade-offs out loud

## Topics
1. Clarifying functional and non-functional requirements
2. Back-of-the-envelope estimation
3. Load balancing, horizontal scaling, statelessness
4. Caching strategies and CDNs
5. Database scaling: replicas, sharding, choosing a store
6. Asynchronous processing and message queues
7. Rate limiting, reliability patterns, graceful degradation
8. Monoliths vs microservices
9. Case studies: URL shortener, news feed, chat, file storage

## Resources
- **Primary:** [The System Design Primer](https://github.com/donnemartin/system-design-primer)
- [roadmap.sh — System Design](https://roadmap.sh/system-design)
- [.NET microservices architecture guide](https://learn.microsoft.com/en-us/dotnet/architecture/microservices/)
- [Azure Architecture Center — cloud design patterns](https://learn.microsoft.com/en-us/azure/architecture/patterns/)
- Books: *Designing Data-Intensive Applications*, *System Design Interview* (Alex Xu)

## Practice
- Write a one-page design for each of the four case studies above, with a diagram and estimates
- Add a cache and a rate limiter to your Module 13 application and measure the effect
- Load-test your application and find its first bottleneck

## Checkpoint project
A **design document** for a system you care about (for example a drawing-management or document-processing service): requirements, estimates, architecture diagram, data model, failure modes, and what you would build first.

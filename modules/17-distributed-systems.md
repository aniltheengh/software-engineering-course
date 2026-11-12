# 17 — Distributed systems

**Duration:** 5 weeks · **Prerequisites:** 08, 09, 10, 14

**Exercises:** [14 exercises for this module](../exercises/17-distributed-systems.md)

## Goals
- Explain why distributed systems fail in the ways they do
- Understand replication, partitioning and consensus at a working level

## Topics
1. Failure models; why networks and clocks cannot be trusted
2. Remote procedure calls and idempotency
3. Replication: leader-follower, multi-leader, leaderless
4. Partitioning and consistent hashing
5. Consistency models; CAP in practice
6. Consensus: Raft
7. Distributed transactions, sagas, outbox pattern
8. Messaging: queues, logs, delivery guarantees
9. Batch and stream processing (overview)

## Resources
- **Primary:** *Designing Data-Intensive Applications* (Kleppmann) — [book site](https://dataintensive.net/)
- [MIT 6.824 / 6.5840 Distributed Systems](https://pdos.csail.mit.edu/6.824/) — lectures and papers
- [The Raft consensus algorithm — paper and visualisation](https://raft.github.io/)
- [Distributed Systems lecture series — Martin Kleppmann (YouTube)](https://www.youtube.com/playlist?list=PLeKd45zvjcDFUEv_ohr_HdUFe97RItdiB)
- [gRPC for .NET](https://learn.microsoft.com/en-us/aspnet/core/grpc/)

## Practice
- Two services communicating over gRPC with retries and idempotency keys
- Consistent-hashing ring with virtual nodes
- Leader election among three local processes
- Read the Raft paper and write a one-page summary

## Checkpoint project
A **replicated key-value store** in C#: three nodes, leader-based replication, survives one node being killed. Full Raft is the stretch goal.

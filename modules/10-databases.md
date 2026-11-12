# 10 — Databases

**Duration:** 5 weeks · **Prerequisites:** 04–05

**Exercises:** [14 exercises for this module](../exercises/10-databases.md)

## Goals
- Design a normalised schema and write non-trivial SQL
- Understand what indexes and transactions do under the hood

## Topics
1. Relational model, keys, normalisation
2. SQL: joins, aggregation, subqueries, window functions
3. Indexes: B-trees, hash indexes, query plans
4. Transactions, ACID, isolation levels
5. Storage engines, buffer pool, write-ahead logging
6. NoSQL families: key-value, document, column, graph
7. Data access from C#: ADO.NET, Dapper, Entity Framework Core, migrations

## Resources
- **Primary:** [CMU 15-445/645 Database Systems](https://15445.courses.cs.cmu.edu/) — lectures on YouTube
- [SQLBolt](https://sqlbolt.com/) — interactive SQL lessons
- [Use The Index, Luke](https://use-the-index-luke.com/) — indexing for developers
- [PostgreSQL tutorial (official docs)](https://www.postgresql.org/docs/current/tutorial.html)
- [Entity Framework Core documentation](https://learn.microsoft.com/en-us/ef/core/)

## Practice
- Finish SQLBolt, then write 30 queries against a sample database
- Design a schema for an equipment/parts catalogue in third normal form
- Use `EXPLAIN ANALYZE` to speed up a slow query by adding an index
- Demonstrate a lost update and fix it with the right isolation level

## Checkpoint project
Rebuild the Module 03 project on **PostgreSQL or SQLite with EF Core**: migrations, seed data, and at least three reporting queries.

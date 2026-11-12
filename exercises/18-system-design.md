# Exercises 18 — System design

Design exercises produce a one- to two-page document: requirements, estimates, diagram, data model, bottlenecks, trade-offs.

### 1. Estimation ★★
A URL shortener receives 100 million new URLs per month with a 10:1 read-to-write ratio. Estimate writes per second, reads per second, and storage for five years at 500 bytes per record.
Expected: about `40` writes/s, `400` reads/s, `3 TB`

### 2. URL shortener ★★
Write the design, then implement it. Encode numeric ids in base 62 using the alphabet `0-9a-zA-Z`.
`125` → `21`

### 3. Rate limiter ★★★
Implement token bucket and sliding-window limiters behind one interface. Test with a fake clock.

### 4. Caching ★★
Add cache-aside to a slow endpoint with `IMemoryCache` or Redis. Measure the hit rate and decide how entries are invalidated on update.

### 5. Load test ★★
Load-test your Module 13 API with k6 or bombardier. Report throughput and 95th-percentile latency, and identify the first bottleneck.

### 6. Load balancing ★★★
Run three instances behind a reverse proxy (nginx or YARP) with round-robin. Kill one during a load test and record what clients see.

### 7. Design: news feed ★★★
Compare fan-out on write with fan-out on read, and choose one for users with very many followers.

### 8. Design: chat ★★★
One-to-one and group messaging with delivery receipts, offline users and message ordering.

### 9. Design: file sync ★★★
A file storage and synchronisation service with chunking, deduplication and conflict handling.

### 10. Design: drawing management ★★★
A service that stores engineering drawings with revisions, approval states, search by tag and number, and preview generation.

### 11. Sharding ★★
Choose a shard key for an `orders` table. Simulate the distribution of one million orders and show what happens with a poorly chosen key.

### 12. Background jobs ★★★
A queue and worker with retries, exponential backoff and a dead-letter queue for jobs that keep failing.

### 13. Storage choice ★★
For three scenarios (financial ledger, product catalogue, sensor time series) write a short memo choosing a type of database and defending the choice.

### 14. Incident reports ★
Read two public post-incident reports from large services. Summarise for each: trigger, root cause, why it spread, what was changed.

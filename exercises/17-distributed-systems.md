# Exercises 17 — Distributed systems

Run several processes on one machine on different ports to stand in for separate nodes.

### 1. gRPC service ★★
A key-value service with `Get`, `Put` and `Delete` over gRPC, plus a console client.

### 2. Idempotency ★★★
Make the server randomly drop responses after applying a request. Add client retries, then add idempotency keys so a retried "increment" is applied exactly once.

### 3. Timeouts and circuit breaker ★★
Add per-call deadlines. Implement a circuit breaker that opens after five consecutive failures and half-opens after ten seconds.

### 4. Lamport clocks ★★
Simulate three processes exchanging messages and stamp every event with a Lamport timestamp. Print the events in timestamp order.

### 5. Vector clocks ★★★
Extend the simulation with vector clocks and report which pairs of events are concurrent.

### 6. Consistent hashing ★★★
Build a hash ring with 100 virtual nodes per server. Place 100,000 keys on three servers, add a fourth, and measure how many keys moved.
Expected: about `25%`

### 7. Replication ★★
One leader and two followers. Implement asynchronous replication and demonstrate a stale read from a follower; then synchronous replication and show the cost in latency.

### 8. Quorums ★★★
With N = 3, W = 2, R = 2, show that reads return the latest write while one node is down, and that writes fail when two are down.

### 9. Leader election ★★★
Three nodes with heartbeats and randomised election timeouts. Kill the leader and show a new one is elected; restart it and show it rejoins as a follower.

### 10. Log replication ★★★
Add Raft-style log replication to exercise 9 so a committed write survives the loss of any single node.

### 11. At-least-once delivery ★★
A producer and consumer through a message queue (RabbitMQ in Docker, or your own). Crash the consumer mid-message and show the message is redelivered and handled once in effect, through deduplication.

### 12. Outbox pattern ★★★
Save an order and publish an "order created" message such that neither can happen without the other, using an outbox table and a relay process.

### 13. Failure injection ★★
Put a proxy between your nodes that adds random delay and drops 10% of messages. List what broke and fix it.

### 14. Paper summary ★★
Read the Raft paper or the Dynamo paper and write a one-page summary: problem, key idea, trade-offs.

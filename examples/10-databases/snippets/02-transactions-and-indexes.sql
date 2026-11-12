-- === SQL: transactions and indexes ===
CREATE TABLE accounts (id INTEGER PRIMARY KEY, owner TEXT NOT NULL, balance NUMERIC NOT NULL CHECK (balance >= 0));
INSERT INTO accounts VALUES (1, 'A', 100), (2, 'B', 50);

-- Both updates happen, or neither does.
BEGIN;
UPDATE accounts SET balance = balance - 30 WHERE id = 1;
UPDATE accounts SET balance = balance + 30 WHERE id = 2;
COMMIT;

-- Changing your mind: ROLLBACK discards everything since BEGIN.
BEGIN;
UPDATE accounts SET balance = 0;
ROLLBACK;

SELECT owner, balance FROM accounts;   -- A 70, B 80

-- An index turns a full table scan into a tree lookup.
CREATE INDEX idx_accounts_owner ON accounts (owner);

-- Ask the database how it will run a query.
-- PostgreSQL:  EXPLAIN ANALYZE SELECT * FROM accounts WHERE owner = 'A';
-- SQLite:      EXPLAIN QUERY PLAN SELECT * FROM accounts WHERE owner = 'A';

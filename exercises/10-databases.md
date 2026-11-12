# Exercises 10 — Databases

Use PostgreSQL or SQLite. Exercises 1–7 use this schema:

```sql
CREATE TABLE customers (
  id   INTEGER PRIMARY KEY,
  name TEXT NOT NULL,
  city TEXT NOT NULL
);
CREATE TABLE products (
  id       INTEGER PRIMARY KEY,
  name     TEXT NOT NULL,
  category TEXT NOT NULL,
  price    NUMERIC NOT NULL
);
CREATE TABLE orders (
  id          INTEGER PRIMARY KEY,
  customer_id INTEGER NOT NULL REFERENCES customers(id),
  ordered_at  DATE NOT NULL
);
CREATE TABLE order_items (
  order_id   INTEGER NOT NULL REFERENCES orders(id),
  product_id INTEGER NOT NULL REFERENCES products(id),
  quantity   INTEGER NOT NULL,
  PRIMARY KEY (order_id, product_id)
);
```

### 1. Seed data ★
Write a script inserting at least 10 customers, 20 products in 4 categories, 50 orders and 120 order items. Leave two customers with no orders.

### 2. Basic queries ★
Products costing more than 50, sorted by price descending. Customers in a given city. The five most recent orders.

### 3. Joins ★★
List every order with the customer's name and the order total (sum of quantity × price).

### 4. Aggregation ★★
Revenue per category. Customers with more than three orders (`HAVING`).

### 5. Missing rows ★★
Customers who have never ordered, written once with `LEFT JOIN` and once with `NOT EXISTS`.

### 6. Subqueries ★★
Products priced above the average of their own category.

### 7. Window functions ★★★
Rank customers by total spend. Show revenue per month with a running total.

### 8. Normalisation ★★
A flat table has the columns `order_id, order_date, customer_name, customer_phone, product_name, product_category, unit_price, quantity`. List its update anomalies and redesign it in third normal form.

### 9. Indexes ★★★
Grow `order_items` to at least one million rows. Find a slow query, read its plan with `EXPLAIN ANALYZE` (or `EXPLAIN QUERY PLAN`), add an index, and record the before and after timings.

### 10. Transactions ★★
Create an `accounts` table and write a transfer between two accounts that either fully succeeds or leaves both balances unchanged. Prove it by forcing a failure between the two updates.

### 11. Isolation ★★★
In two database sessions, reproduce a lost update. Prevent it once with a stricter isolation level and once with `SELECT ... FOR UPDATE` or optimistic versioning.

### 12. Data access from C# ★★
Run exercises 3 and 4 from C# with ADO.NET or Dapper using parameterised queries only.

### 13. EF Core ★★
Model the schema as EF Core entities, create it with a migration, and rewrite exercises 3–6 as LINQ queries. Compare the SQL that EF Core generates with your own.

### 14. N+1 queries ★★★
Write code that loads orders and then touches each order's items in a loop. Log the SQL to show the N+1 problem, then fix it with `Include` or a projection.

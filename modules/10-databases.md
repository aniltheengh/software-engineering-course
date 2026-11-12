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

## Code examples

All of the C# below is one runnable project in [examples/10-databases](../examples/10-databases). Run it with `dotnet run` from that folder, then change it and run it again.

### Select, join, group and outer join

```csharp
static class QueryDemo
{
    public static void Run()
    {
        // SELECT name FROM customers WHERE city = 'Tokyo' ORDER BY name
        var tokyo = Data.Customers.Where(c => c.City == "Tokyo").OrderBy(c => c.Name).Select(c => c.Name);
        Console.WriteLine(string.Join(", ", tokyo));   // Aiko, Chandra

        // JOIN three tables and aggregate: total per order
        var totals =
            from o in Data.Orders
            join c in Data.Customers on o.CustomerId equals c.Id
            join i in Data.Items on o.Id equals i.OrderId
            join p in Data.Products on i.ProductId equals p.Id
            group i.Quantity * p.Price by new { o.Id, c.Name } into g
            select new { g.Key.Id, g.Key.Name, Total = g.Sum() };
        foreach (var row in totals) Console.WriteLine($"order {row.Id} ({row.Name}): {row.Total}");   // 110, 55, 150

        // GROUP BY category
        var perCategory =
            from i in Data.Items
            join p in Data.Products on i.ProductId equals p.Id
            group i.Quantity * p.Price by p.Category into g
            select new { Category = g.Key, Revenue = g.Sum() };
        foreach (var row in perCategory) Console.WriteLine($"{row.Category}: {row.Revenue}");   // Valves 135, Seals 180

        // LEFT JOIN ... WHERE orders.id IS NULL: customers with no orders
        var noOrders = Data.Customers.Where(c => !Data.Orders.Any(o => o.CustomerId == c.Id)).Select(c => c.Name);
        Console.WriteLine(string.Join(", ", noOrders));   // Chandra
    }
}
```

### Transactions: all or nothing

```csharp
static class TransactionDemo
{
    public static void Run()
    {
        var balances = new Dictionary<string, decimal> { ["A"] = 100m, ["B"] = 50m };

        bool Transfer(string from, string to, decimal amount)
        {
            var snapshot = new Dictionary<string, decimal>(balances);   // BEGIN
            try
            {
                balances[from] -= amount;
                if (balances[from] < 0) throw new InvalidOperationException("insufficient funds");
                balances[to] += amount;
                return true;                                            // COMMIT
            }
            catch (InvalidOperationException)
            {
                foreach (var (key, value) in snapshot) balances[key] = value;   // ROLLBACK
                return false;
            }
        }

        Console.WriteLine(Transfer("A", "B", 30m));    // True
        Console.WriteLine(Transfer("A", "B", 500m));   // False, and nothing changed
        Console.WriteLine($"A={balances["A"]} B={balances["B"]}");   // A=70 B=80
    }
}
```

### What an index does

```csharp
static class IndexDemo
{
    public static void Run()
    {
        var rows = Enumerable.Range(1, 500_000).Select(i => new Customer(i, "Customer " + i, "City " + i % 100)).ToList();

        var timer = System.Diagnostics.Stopwatch.StartNew();
        for (int i = 0; i < 50; i++) rows.First(r => r.Id == 499_999);   // full scan, like a query with no index
        Console.WriteLine($"scan:  {timer.ElapsedMilliseconds} ms");

        var index = rows.ToDictionary(r => r.Id);   // like CREATE INDEX: extra space, extra work on every insert
        timer.Restart();
        for (int i = 0; i < 50; i++) _ = index[499_999];
        Console.WriteLine($"index: {timer.ElapsedMilliseconds} ms");
    }
}
```

### SQL: schema, data and queries

From [`snippets/01-queries.sql`](../examples/10-databases/snippets/01-queries.sql).

```sql
CREATE TABLE customers (id INTEGER PRIMARY KEY, name TEXT NOT NULL, city TEXT NOT NULL);
CREATE TABLE products  (id INTEGER PRIMARY KEY, name TEXT NOT NULL, category TEXT NOT NULL, price NUMERIC NOT NULL);
CREATE TABLE orders    (id INTEGER PRIMARY KEY, customer_id INTEGER NOT NULL REFERENCES customers(id), ordered_at DATE NOT NULL);
CREATE TABLE order_items (
  order_id   INTEGER NOT NULL REFERENCES orders(id),
  product_id INTEGER NOT NULL REFERENCES products(id),
  quantity   INTEGER NOT NULL CHECK (quantity > 0),
  PRIMARY KEY (order_id, product_id)
);

INSERT INTO customers VALUES (1, 'Aiko', 'Tokyo'), (2, 'Ben', 'Osaka'), (3, 'Chandra', 'Tokyo');
INSERT INTO products  VALUES (1, 'Ball valve', 'Valves', 40), (2, 'Gate valve', 'Valves', 55), (3, 'Gasket', 'Seals', 3);
INSERT INTO orders    VALUES (100, 1, '2026-01-05'), (101, 1, '2026-02-09'), (102, 2, '2026-02-11');
INSERT INTO order_items VALUES (100, 1, 2), (100, 3, 10), (101, 2, 1), (102, 3, 50);

-- Filter and sort
SELECT name FROM customers WHERE city = 'Tokyo' ORDER BY name;

-- Join and aggregate: total per order            -> 100: 110, 101: 55, 102: 150
SELECT o.id, c.name, SUM(i.quantity * p.price) AS total
FROM orders o
JOIN customers c   ON c.id = o.customer_id
JOIN order_items i ON i.order_id = o.id
JOIN products p    ON p.id = i.product_id
GROUP BY o.id, c.name
ORDER BY o.id;

-- Group and filter the groups                    -> Seals: 180
SELECT p.category, SUM(i.quantity * p.price) AS revenue
FROM order_items i JOIN products p ON p.id = i.product_id
GROUP BY p.category
HAVING SUM(i.quantity * p.price) > 150;

-- Outer join: customers with no orders           -> Chandra
SELECT c.name
FROM customers c LEFT JOIN orders o ON o.customer_id = c.id
WHERE o.id IS NULL;

-- Window function: rank customers by spend       -> Aiko 1, Ben 2
SELECT c.name,
       SUM(i.quantity * p.price) AS spend,
       RANK() OVER (ORDER BY SUM(i.quantity * p.price) DESC) AS position
FROM customers c
JOIN orders o      ON o.customer_id = c.id
JOIN order_items i ON i.order_id = o.id
JOIN products p    ON p.id = i.product_id
GROUP BY c.id, c.name;
```

### SQL: transactions and indexes

From [`snippets/02-transactions-and-indexes.sql`](../examples/10-databases/snippets/02-transactions-and-indexes.sql).

```sql
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
```

### Database access from C#: ADO.NET and EF Core

From [`snippets/03-DataAccess.cs`](../examples/10-databases/snippets/03-DataAccess.cs).

```csharp
// Needs NuGet packages: Microsoft.Data.Sqlite and Microsoft.EntityFrameworkCore.Sqlite
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

// --- ADO.NET: you write the SQL -------------------------------------------
using (var connection = new SqliteConnection("Data Source=shop.db"))
{
    connection.Open();

    using var command = connection.CreateCommand();
    command.CommandText = "SELECT id, name FROM customers WHERE city = $city";
    command.Parameters.AddWithValue("$city", "Tokyo");   // a parameter, never string concatenation

    using var reader = command.ExecuteReader();
    while (reader.Read())
        Console.WriteLine($"{reader.GetInt32(0)}: {reader.GetString(1)}");
}

// --- EF Core: you write LINQ, it writes the SQL ---------------------------
using (var db = new ShopContext())
{
    db.Database.EnsureCreated();

    db.Customers.Add(new Customer { Name = "Dai", City = "Nagoya" });
    db.SaveChanges();                                    // one transaction

    var tokyo = db.Customers
        .Where(c => c.City == "Tokyo")
        .OrderBy(c => c.Name)
        .ToList();                                       // the query runs here

    var withOrders = db.Customers
        .Include(c => c.Orders)                          // load related rows in the same query
        .ToList();
}

public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string City { get; set; } = "";
    public List<Order> Orders { get; set; } = new();
}

public class Order
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public DateOnly OrderedAt { get; set; }
}

public class ShopContext : DbContext
{
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Order> Orders => Set<Order>();

    protected override void OnConfiguring(DbContextOptionsBuilder options) =>
        options.UseSqlite("Data Source=shop.db");
}
```

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

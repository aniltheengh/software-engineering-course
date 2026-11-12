// Run with: dotnet run
// The relational operations from SQL, shown with LINQ over in-memory lists.
// The SQL versions are in the snippets folder.
QueryDemo.Run();
TransactionDemo.Run();
IndexDemo.Run();

record Customer(int Id, string Name, string City);
record Product(int Id, string Name, string Category, decimal Price);
record Order(int Id, int CustomerId, DateOnly OrderedAt);
record OrderItem(int OrderId, int ProductId, int Quantity);

static class Data
{
    public static readonly List<Customer> Customers = new()
    {
        new(1, "Aiko", "Tokyo"), new(2, "Ben", "Osaka"), new(3, "Chandra", "Tokyo")
    };
    public static readonly List<Product> Products = new()
    {
        new(1, "Ball valve", "Valves", 40m), new(2, "Gate valve", "Valves", 55m), new(3, "Gasket", "Seals", 3m)
    };
    public static readonly List<Order> Orders = new()
    {
        new(100, 1, new DateOnly(2026, 1, 5)), new(101, 1, new DateOnly(2026, 2, 9)), new(102, 2, new DateOnly(2026, 2, 11))
    };
    public static readonly List<OrderItem> Items = new()
    {
        new(100, 1, 2), new(100, 3, 10), new(101, 2, 1), new(102, 3, 50)
    };
}

// === Select, join, group and outer join ===
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

// === Transactions: all or nothing ===
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

// === What an index does ===
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

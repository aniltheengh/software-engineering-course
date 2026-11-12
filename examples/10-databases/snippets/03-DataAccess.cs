// === Database access from C#: ADO.NET and EF Core ===
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

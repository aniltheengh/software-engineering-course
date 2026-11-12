// === SQL injection and the fix ===
// Needs NuGet package: Microsoft.Data.Sqlite
using Microsoft.Data.Sqlite;

string userInput = "' OR '1'='1";   // what an attacker types into the name box

using var connection = new SqliteConnection("Data Source=app.db");
connection.Open();

// VULNERABLE: the input becomes part of the SQL text.
// The query turns into:  SELECT * FROM users WHERE name = '' OR '1'='1'   -> every row
using (var bad = connection.CreateCommand())
{
    bad.CommandText = "SELECT * FROM users WHERE name = '" + userInput + "'";
}

// SAFE: the SQL text is fixed; the input travels separately as data.
using (var good = connection.CreateCommand())
{
    good.CommandText = "SELECT * FROM users WHERE name = $name";
    good.Parameters.AddWithValue("$name", userInput);   // matches no user
    using var reader = good.ExecuteReader();
}

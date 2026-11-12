# 15 — Security

**Duration:** 3 weeks · **Prerequisites:** 09, 13

**Exercises:** [14 exercises for this module](../exercises/15-security.md)

## Goals
- Recognise and prevent the common classes of vulnerability in your own code
- Use cryptography correctly by relying on vetted libraries

## Topics
1. Threat modelling
2. OWASP Top 10: injection, broken access control, XSS, CSRF and the rest
3. Cryptography concepts: hashing, symmetric and asymmetric encryption, signatures
4. Password storage and authentication flows
5. TLS in practice
6. Secrets management
7. Dependency and supply-chain security
8. Secure coding guidelines for .NET

## Code examples

All of the C# below is one runnable project in [examples/15-security](../examples/15-security). Run it with `dotnet run` from that folder, then change it and run it again.

### Hashing: a fixed-size fingerprint of any data

```csharp
static class HashingDemo
{
    public static void Run()
    {
        string Hash(string text) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(text)));

        Console.WriteLine(Hash("hello"));   // 2CF24DBA5FB0A30E26E83B2AC5B9E29E1B161E5C1FA7425E73043362938B9824
        Console.WriteLine(Hash("hellp"));   // one letter changed: a completely different hash
        // A hash cannot be reversed, but a plain fast hash is NOT enough for passwords.
    }
}
```

### Storing passwords: slow hash, random salt, constant-time compare

```csharp
static class PasswordDemo
{
    const int Iterations = 210_000;

    static (byte[] Salt, byte[] Hash) HashPassword(string password)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(16);   // different for every user
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, 32);
        return (salt, hash);
    }

    static bool Verify(string password, byte[] salt, byte[] expected)
    {
        byte[] actual = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, 32);
        return CryptographicOperations.FixedTimeEquals(actual, expected);   // no timing leak
    }

    public static void Run()
    {
        var (salt, hash) = HashPassword("correct horse battery staple");
        Console.WriteLine(Verify("correct horse battery staple", salt, hash));   // True
        Console.WriteLine(Verify("guess", salt, hash));                          // False
        // Store the salt, the hash and the iteration count. Never store the password.
    }
}
```

### Authenticated encryption with AES-GCM

```csharp
static class EncryptionDemo
{
    public static void Run()
    {
        byte[] key = RandomNumberGenerator.GetBytes(32);     // 256-bit key: keep it secret
        byte[] nonce = RandomNumberGenerator.GetBytes(12);   // unique per message, not secret
        byte[] plaintext = Encoding.UTF8.GetBytes("valve list rev C");
        byte[] ciphertext = new byte[plaintext.Length];
        byte[] tag = new byte[16];

        using var aes = new AesGcm(key, tagSizeInBytes: 16);
        aes.Encrypt(nonce, plaintext, ciphertext, tag);
        Console.WriteLine(Convert.ToBase64String(ciphertext));

        byte[] decrypted = new byte[ciphertext.Length];
        aes.Decrypt(nonce, ciphertext, tag, decrypted);
        Console.WriteLine(Encoding.UTF8.GetString(decrypted));   // valve list rev C

        ciphertext[0] ^= 1;   // an attacker flips one bit
        try { aes.Decrypt(nonce, ciphertext, tag, decrypted); }
        catch (CryptographicException) { Console.WriteLine("tampering detected: decryption refused"); }
    }
}
```

### Digital signatures: prove who wrote it and that it is unchanged

```csharp
static class SigningDemo
{
    public static void Run()
    {
        using ECDsa key = ECDsa.Create(ECCurve.NamedCurves.nistP256);
        byte[] document = Encoding.UTF8.GetBytes("approved for construction");

        byte[] signature = key.SignData(document, HashAlgorithmName.SHA256);          // needs the private key
        Console.WriteLine(key.VerifyData(document, signature, HashAlgorithmName.SHA256));   // True

        byte[] altered = Encoding.UTF8.GetBytes("approved for demolition");
        Console.WriteLine(key.VerifyData(altered, signature, HashAlgorithmName.SHA256));    // False
    }
}
```

### Output encoding stops cross-site scripting

```csharp
static class OutputEncodingDemo
{
    public static void Run()
    {
        string comment = "<script>alert('stolen')</script>";

        Console.WriteLine($"<p>{comment}</p>");                         // unsafe: the browser would run it
        Console.WriteLine($"<p>{WebUtility.HtmlEncode(comment)}</p>");  // safe: shown as text
    }
}
```

### Path traversal: never trust a file name from a request

```csharp
static class PathDemo
{
    static bool IsInside(string baseFolder, string requestedName)
    {
        string root = Path.GetFullPath(baseFolder) + Path.DirectorySeparatorChar;
        string full = Path.GetFullPath(Path.Combine(baseFolder, requestedName));   // resolves ".." segments
        return full.StartsWith(root, StringComparison.Ordinal);
    }

    public static void Run()
    {
        string folder = Path.Combine(Path.GetTempPath(), "downloads");
        Console.WriteLine(IsInside(folder, "report.pdf"));                 // True
        Console.WriteLine(IsInside(folder, "../../etc/passwd"));           // False
        Console.WriteLine(IsInside(folder, "sub/../report.pdf"));          // True: still inside
    }
}
```

### SQL injection and the fix

From [`snippets/01-SqlInjection.cs`](../examples/15-security/snippets/01-SqlInjection.cs).

```csharp
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
```

## Resources
- **Primary:** [OWASP Top Ten](https://owasp.org/www-project-top-ten/)
- [OWASP Cheat Sheet Series](https://cheatsheetseries.owasp.org/)
- [ASP.NET Core security documentation](https://learn.microsoft.com/en-us/aspnet/core/security/)
- [PortSwigger Web Security Academy (free labs)](https://portswigger.net/web-security)
- [Cryptography I — Dan Boneh (Coursera)](https://www.coursera.org/learn/crypto)

## Practice
- Complete the SQL injection, XSS and access-control lab tracks on Web Security Academy
- Write a deliberately injectable query in a throwaway app, exploit it, then parameterise it
- Hash passwords with a proper password-hashing function and verify them
- Move all secrets in your Module 13 app to user secrets / environment variables

## Checkpoint project
A **security review** of your Module 13 application: a threat model, a findings list against the OWASP Top 10, and the fixes committed.

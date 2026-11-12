# Exercises 11 — Testing and code quality

Use xUnit throughout.

### 1. First tests ★
Write `IsLeapYear(int year)` and tests for it.
`2000` → true, `1900` → false, `2024` → true, `2023` → false

### 2. Theories ★
Test FizzBuzz with a single `[Theory]` and `[InlineData]` rows covering every branch.

### 3. Testing exceptions ★
Test that your Module 03 `BankAccount` throws on overdraft and on non-positive amounts, and that the balance is unchanged afterwards.

### 4. String calculator kata ★★
With strict test-driven development (red, green, refactor), build `Add(string numbers)`: empty string gives 0, numbers are separated by commas or newlines, negative numbers throw an exception listing all of them. Commit after every green step.

### 5. Bowling kata ★★★
Score a ten-pin bowling game by TDD.
All strikes → `300`; twenty-one rolls of 5 → `150`; all gutter balls → `0`

### 6. Test doubles ★★
`OrderService.PlaceOrder` sends a confirmation through `IEmailSender` and stamps the order using `IClock`. Test it first with hand-written fakes, then with a mocking library.

### 7. Making code testable ★★
Refactor this method so it can be unit tested without touching the real clock or file system, then test it.

```csharp
public static string Greeting()
{
    var name = File.ReadAllText("user.txt").Trim();
    var hour = DateTime.Now.Hour;
    return (hour < 12 ? "Good morning, " : "Good afternoon, ") + name;
}
```

### 8. Gilded Rose ★★★
Take the Gilded Rose refactoring kata (C# version). Write characterisation tests that pin down the current behaviour, then refactor until adding a new item type is a small change.

### 9. Coverage ★★
Run `dotnet test --collect:"XPlat Code Coverage"` on an earlier project. Find three untested branches and decide for each whether a test is worth writing.

### 10. Integration test ★★
Test a repository class against a real SQLite in-memory database rather than a mock.

### 11. Property-style test ★★★
For 1,000 random arrays, assert that your Module 05 sort produces output that is ordered and is a permutation of its input.

### 12. Analyzers ★★
Add `<AnalysisLevel>latest-recommended</AnalysisLevel>` and `<TreatWarningsAsErrors>true</TreatWarningsAsErrors>` to a project and fix everything it reports. Add `dotnet format --verify-no-changes` as a check.

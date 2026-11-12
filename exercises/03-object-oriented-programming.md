# Exercises 03 — Object-oriented programming

### 1. Rectangle ★
A `Rectangle` class with `Width`, `Height`, `Area` and `Perimeter`. Creating one with a non-positive side must throw `ArgumentOutOfRangeException`.

### 2. Temperature value type ★
An immutable `Temperature` struct created from Celsius, Fahrenheit or Kelvin, with conversion properties, equality and comparison (`IComparable<Temperature>`).

### 3. Bank account ★
`Deposit`, `Withdraw`, `Balance`. The balance can never go negative, amounts must be positive, and the transaction history is exposed as read-only.

### 4. Shapes by interface ★★
`IShape` with `Area`. Implement `Circle`, `Rectangle`, `Triangle`. Write one method that returns the total area of any list of shapes.

### 5. Payroll ★★
Abstract `Employee` with `CalculatePay()`. `SalariedEmployee` and `HourlyEmployee` (overtime at 1.5× above 40 hours) override it. Print a payroll for a mixed list.

### 6. Composition over inheritance ★★
Start with `class Stack<T> : List<T>` and show a way to misuse it. Rewrite it so the list is a private field and misuse is impossible.

### 7. Generics ★★
Write `Pair<TFirst, TSecond>` and a static `Max<T>(IEnumerable<T>) where T : IComparable<T>`. Make `Max` work for your `Temperature` struct.

### 8. Records ★★
Define `Point` as a record and as a class. Demonstrate the difference in equality, `ToString` and `with` expressions.

### 9. Fractions ★★★
A `Fraction` type that is always stored in lowest terms with a positive denominator, with `+ - * /`, `==` and `!=`.
`1/2 + 1/3` → `5/6`

### 10. Events ★★★
A `Thermostat` raises `TemperatureChanged` whenever its value changes by more than 0.5 degrees. Attach a logger and an alarm as subscribers, then unsubscribe one and show it no longer fires.

### 11. Disposable resource ★★★
A `TempFile` class creates a temporary file in its constructor and deletes it in `Dispose`. Prove with a `using` block that the file is gone afterwards, even when an exception is thrown inside the block.

### 12. Swappable storage ★★★
Define `IRepository<T>` with `Add`, `GetById`, `GetAll`, `Remove`. Implement `InMemoryRepository<T>` and `JsonFileRepository<T>`. The rest of the program must run unchanged with either.

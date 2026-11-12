# 06 — Discrete mathematics

**Duration:** 4 weeks · **Prerequisites:** school-level algebra (run in parallel with 04–05 if you prefer)

**Exercises:** [12 exercises for this module](../exercises/06-discrete-mathematics.md)

## Goals
- Read and write simple proofs
- Use logic, counting and graph theory when reasoning about programs

## Topics
1. Propositional and predicate logic
2. Proof techniques: direct, contradiction, induction
3. Sets, relations, functions
4. Number theory basics: modular arithmetic, GCD, primes
5. Counting: permutations, combinations, pigeonhole
6. Graph theory: paths, trees, colouring
7. Discrete probability and expectation
8. Asymptotics and sums

## Code examples

All of the C# below is one runnable project in [examples/06-discrete-mathematics](../examples/06-discrete-mathematics). Run it with `dotnet run` from that folder, then change it and run it again.

### Logic: a truth table and an equivalence check

```csharp
static class LogicDemo
{
    public static void Run()
    {
        // "p implies q" is false only when p is true and q is false.
        Func<bool, bool, bool> implies = (p, q) => !p || q;

        Console.WriteLine("p     q     p->q  !q->!p");
        foreach (bool p in new[] { true, false })
            foreach (bool q in new[] { true, false })
                Console.WriteLine($"{p,-5} {q,-5} {implies(p, q),-5} {implies(!q, !p),-5}");
        // The last two columns match in every row: a statement equals its contrapositive.
    }
}
```

### Sets and relations

```csharp
static class SetsDemo
{
    public static void Run()
    {
        var a = new HashSet<int> { 1, 2, 3, 4 };
        var b = new HashSet<int> { 3, 4, 5 };

        Console.WriteLine(string.Join(" ", a.Union(b)));       // 1 2 3 4 5
        Console.WriteLine(string.Join(" ", a.Intersect(b)));   // 3 4
        Console.WriteLine(string.Join(" ", a.Except(b)));      // 1 2
        Console.WriteLine(new HashSet<int> { 3, 4 }.IsSubsetOf(a));   // True

        // Cartesian product A x B: every ordered pair.
        var pairs = from x in new[] { 1, 2 } from y in new[] { "a", "b" } select (x, y);
        Console.WriteLine(string.Join(" ", pairs));            // (1, a) (1, b) (2, a) (2, b)
    }
}
```

### Number theory: GCD and modular arithmetic

```csharp
static class NumberTheoryDemo
{
    static int Gcd(int a, int b) => b == 0 ? a : Gcd(b, a % b);   // Euclid's algorithm

    public static void Run()
    {
        Console.WriteLine(Gcd(84, 36));            // 12
        Console.WriteLine(84 / Gcd(84, 36) * 36);  // 252   least common multiple

        Console.WriteLine((17 + 9) % 12);          // 2     clock arithmetic
        Console.WriteLine(-7 % 3);                 // -1    C# keeps the sign of the left operand
        Console.WriteLine(((-7 % 3) + 3) % 3);     // 2     the mathematical result

        // (a * b) mod m equals ((a mod m) * (b mod m)) mod m: keeps numbers small.
        long a = 123_456_789, b = 987_654_321, m = 1_000_000_007;
        Console.WriteLine(a % m * (b % m) % m);    // 259106859
    }
}
```

### Counting: factorials, permutations, combinations

```csharp
static class CountingDemo
{
    static BigInteger Factorial(int n)
    {
        BigInteger result = 1;
        for (int i = 2; i <= n; i++) result *= i;
        return result;
    }

    static BigInteger Permutations(int n, int k) => Factorial(n) / Factorial(n - k);          // order matters
    static BigInteger Combinations(int n, int k) => Permutations(n, k) / Factorial(k);        // order ignored

    public static void Run()
    {
        Console.WriteLine(Factorial(20));          // 2432902008176640000
        Console.WriteLine(Permutations(10, 3));    // 720   ways to award gold, silver, bronze to 10 people
        Console.WriteLine(Combinations(10, 3));    // 120   ways to pick a team of 3 from 10
        Console.WriteLine(BigInteger.Pow(2, 10));  // 1024  subsets of a 10-element set
    }
}
```

### Induction, checked by program

```csharp
// Claim: 1 + 2 + ... + n = n(n+1)/2.  A program cannot prove it, but it can find a counter-example.
static class InductionDemo
{
    public static void Run()
    {
        long sum = 0;
        bool holds = true;
        for (long n = 1; n <= 100_000; n++)
        {
            sum += n;
            if (sum != n * (n + 1) / 2) { holds = false; break; }
        }
        Console.WriteLine($"formula holds for n = 1..100000: {holds}");
        // Proof: base case n=1 gives 1. Step: if it holds for n, then adding (n+1) gives
        // n(n+1)/2 + (n+1) = (n+1)(n+2)/2, which is the formula for n+1.
    }
}
```

### Probability by simulation

```csharp
static class ProbabilityDemo
{
    public static void Run()
    {
        var random = new Random(42);   // a fixed seed makes the run repeatable
        const int trials = 1_000_000;
        int sevens = 0;
        for (int i = 0; i < trials; i++)
            if (random.Next(1, 7) + random.Next(1, 7) == 7) sevens++;

        Console.WriteLine($"simulated P(sum of two dice = 7) = {(double)sevens / trials:F4}");
        Console.WriteLine($"exact: 6/36 = {6.0 / 36:F4}");   // 0.1667
    }
}
```

## Resources
- **Primary:** [MIT 6.042J Mathematics for Computer Science (OCW)](https://ocw.mit.edu/courses/6-042j-mathematics-for-computer-science-fall-2010/) — lectures and free textbook
- [Discrete Mathematics: An Open Introduction (free book)](https://discrete.openmathbooks.org/dmoi3.html)

## Practice
- Problem sets 1–6 from 6.042J
- Write C# programs for: sieve of Eratosthenes, extended Euclid, modular exponentiation, generating permutations and combinations
- Prove the correctness of binary search by induction

## Checkpoint project
A short written set of **five proofs** (Markdown or LaTeX) plus a small C# number-theory library with the functions above.

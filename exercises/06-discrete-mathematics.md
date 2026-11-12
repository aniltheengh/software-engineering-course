# Exercises 06 — Discrete mathematics

Written exercises go in Markdown files; programming exercises in C#.

### 1. Truth tables ★
Write a program that prints the truth table of a three-variable Boolean function. Use it to verify both De Morgan laws.

### 2. Induction ★★
Prove by induction that 1 + 2 + … + n = n(n + 1)/2. Then prove that the sum of the first n odd numbers is n².

### 3. Contradiction ★★
Prove that √2 is irrational.

### 4. Sets ★★
Implement union, intersection, difference and power set for sets of strings.
The power set of `{a, b, c}` has `8` elements.

### 5. Euclid ★★
Implement GCD and the extended Euclidean algorithm.
`gcd(252, 198) = 18`, with `252·4 + 198·(−5) = 18`

### 6. Modular exponentiation ★★
Compute a^b mod m by repeated squaring, without `BigInteger.ModPow`.
`3^200 mod 13 = 9`

### 7. Sieve of Eratosthenes ★
List all primes below N. There are `25` primes below 100.

### 8. Binomial coefficients ★★
Compute C(n, k) without overflowing on intermediate values.
`C(52, 5) = 2,598,960`

### 9. Pigeonhole ★★
Prove: among any 13 people, two were born in the same month. Prove: among any five integers, two have the same remainder when divided by 4.

### 10. Graphs ★★★
Prove that the sum of all vertex degrees equals twice the number of edges. Then write a program that decides whether a graph is bipartite by 2-colouring it with BFS.

### 11. Birthday paradox ★★
Compute exactly, and estimate by simulation with 100,000 trials, the probability that at least two of 23 people share a birthday.
Expected: about `0.507`

### 12. Coupon collector ★★★
Derive the expected number of rolls of a fair die until all six faces have appeared, then confirm it by simulation.
Expected: `14.7`

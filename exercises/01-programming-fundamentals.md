# Exercises 01 — Programming fundamentals in C#

### 1. Temperature table ★
Print Celsius from 0 to 100 in steps of 10 with the Fahrenheit value beside each, in aligned columns with one decimal place.

### 2. FizzBuzz ★
Print 1 to 100. Multiples of 3 print `Fizz`, multiples of 5 print `Buzz`, multiples of both print `FizzBuzz`.

### 3. Sum of digits ★
Read an integer and print the sum of its digits. Must work for negative numbers.
`9875` → `29`

### 4. Reverse a string ★
Reverse a string without `Array.Reverse` or LINQ `Reverse`.
`hello` → `olleh`

### 5. Primes below N ★★
Write `bool IsPrime(int n)` and use it to list all primes below N.
`N = 30` → `2 3 5 7 11 13 17 19 23 29`

### 6. Guessing game ★★
The program picks a random number from 1 to 100. The user guesses; the program answers "higher" or "lower" and counts attempts. Non-numeric input must not crash it and must not count as an attempt.

### 7. Array statistics ★★
Read a line of numbers separated by spaces. Print min, max, mean, median and standard deviation.
`4 8 15 16 23 42` → min 4, max 42, mean 18, median 15.5

### 8. Palindrome check ★★
Decide whether a sentence is a palindrome, ignoring case, spaces and punctuation.
`A man, a plan, a canal: Panama` → `true`

### 9. Word frequency ★★
Read a text file and print the 10 most frequent words with their counts, case-insensitive, using `Dictionary<string,int>`.

### 10. Roman numerals ★★★
Convert integers 1–3999 to Roman numerals and back.
`1994` ↔ `MCMXCIV`

### 11. CSV grade report ★★★
Input file lines look like `name,score1,score2,score3`. Print each student's average sorted from highest to lowest, then the class average. Malformed lines are skipped with a warning that includes the line number.

### 12. LINQ queries ★★★
Create a list of at least 20 `Order(Customer, Product, Quantity, UnitPrice, Date)` records. Using LINQ only, compute: total revenue per customer; the three best-selling products by quantity; revenue per month; customers who bought more than one distinct product.

# Exercises 19 — Programming languages and compilers

Exercises 1–10 grow one small language step by step.

### 1. Tokenizer ★
Turn `3 + 4.5 * (2 - 1)` into a list of tokens with type, text and position.

### 2. Parser and evaluator ★★
Recursive-descent parser for `+ - * / ^`, parentheses and unary minus, with correct precedence. `^` is right-associative.
`2 + 3 * 4` → `14`; `(2 + 3) * 4` → `20`; `2 ^ 3 ^ 2` → `512`

### 3. Syntax tree ★★
Make the parser build an abstract syntax tree from records, and write a printer that shows it with full parentheses.
`2 + 3 * 4` → `(2 + (3 * 4))`

### 4. Visitors ★★
Implement evaluation, printing and node counting as three visitors over the same tree.

### 5. Variables ★★
Add assignment and variable lookup with an environment. Using an undefined variable is an error.

### 6. Error reporting ★★
Report syntax errors with line and column and a caret under the offending token. Recover enough to report more than one error per run.

### 7. Statements ★★★
Add `print`, `if`/`else` and `while`. Write a program in your language that prints the factorial of 10.
Expected: `3628800`

### 8. Functions ★★★
Add function definitions, calls, `return`, recursion and closures. Write recursive Fibonacci in your language.

### 9. Bytecode ★★★
Compile the tree to instructions for the stack machine from Module 07 and run them there.

### 10. Constant folding ★★
An optimisation pass that rewrites `2 * 3 + x` to `6 + x` before code generation.

### 11. Shunting-yard ★★
Convert infix expressions to reverse Polish notation and evaluate them with a stack.
`3 + 4 * 2` → `3 4 2 * +` → `11`

### 12. JSON parser ★★★
Parse JSON into your own object model with no library, including nested objects, arrays, escapes and numbers with exponents.

### 13. Roslyn ★★★
Walk the syntax tree of a `.cs` file and print every method with its line count. Then write an analyzer that flags empty `catch` blocks.

### 14. F# ★★
Rewrite the expression evaluator from exercise 3 in F# using a discriminated union and pattern matching. Compare its length with the C# version.

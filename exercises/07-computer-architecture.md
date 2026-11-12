# Exercises 07 — Computer architecture

### 1. Base conversion ★
Convert between decimal, binary and hexadecimal with your own code (no `Convert.ToString(n, 2)`).
`156` → `10011100` → `0x9C`

### 2. Two's complement ★
Print the 8-bit two's complement pattern of a number from −128 to 127.
`-37` → `11011011`

### 3. Bit manipulation ★★
Write functions that: count set bits; test whether a number is a power of two; swap two integers without a temporary; extract bits 4–7 of a byte.

### 4. Inside a float ★★
Using `BitConverter.SingleToInt32Bits`, print the sign, biased exponent and mantissa of a `float`.
`0.15625` → sign `0`, exponent `124`, mantissa `0x200000`

### 5. Floating-point error ★★
Show that `0.1 + 0.2 != 0.3`. Add `0.1` ten million times using `float`, `double` and `decimal` and compare with the exact answer. Then fix the `float` result with Kahan summation.

### 6. Gates from NAND ★★
Starting from a single `Nand(bool, bool)` function, build NOT, AND, OR, XOR, a half adder, a full adder and an 8-bit ripple-carry adder. Test the adder against `+` for all 65,536 input pairs.

### 7. Stack machine ★★★
Write a virtual machine with `PUSH`, `ADD`, `SUB`, `MUL`, `DUP`, `SWAP`, `JMP`, `JZ`, `PRINT`, `HALT`. Write a program in its instruction set that computes 5 factorial.
Expected output: `120`

### 8. Cache effects ★★
Sum an 8,000 × 8,000 `int[,]` row by row, then column by column. Time both and explain the difference.

### 9. Struct vs class ★★
Allocate an array of one million points, once as a `struct` and once as a `class`. Compare memory use (`GC.GetTotalMemory`) and iteration time.

### 10. Allocation-free parsing ★★★
Parse a line of comma-separated integers and sum them using `ReadOnlySpan<char>`, with no string allocations. Confirm with `GC.GetAllocatedBytesForCurrentThread`.

### 11. Reading IL ★★
Compile a small loop and inspect its IL with `ildasm`, ILSpy or [SharpLab](https://sharplab.io). Annotate each instruction.

### 12. Stack depth ★★
Find roughly how deep a recursive method can go before `StackOverflowException`. Rewrite the same computation iteratively with an explicit stack.

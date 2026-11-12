# 07 — Computer architecture

**Duration:** 5 weeks · **Prerequisites:** 01–05

**Exercises:** [12 exercises for this module](../exercises/07-computer-architecture.md)

## Goals
- Explain how a program becomes instructions executed by hardware
- Reason about memory layout, caches and performance

## Topics
1. Binary, two's complement, floating point (IEEE 754)
2. Logic gates, adders, memory elements
3. CPU: instruction set, fetch-decode-execute, pipelining
4. Assembly basics and the call stack
5. Memory hierarchy and cache behaviour
6. Stack vs heap; how the .NET runtime, JIT and garbage collector fit in
7. Linking and loading

## Code examples

All of the C# below is one runnable project in [examples/07-computer-architecture](../examples/07-computer-architecture). Run it with `dotnet run` from that folder, then change it and run it again.

### Binary, hexadecimal and bitwise operators

```csharp
static class BitsDemo
{
    static string Bin(int value, int width = 8) => Convert.ToString(value, 2).PadLeft(width, '0');

    public static void Run()
    {
        int a = 0b1100_1010;   // binary literal = 202
        int b = 0x0F;          // hex literal    = 15

        Console.WriteLine($"{a} = {Bin(a)} = 0x{a:X2}");
        Console.WriteLine($"a & b  = {Bin(a & b)}");    // 00001010  AND keeps bits set in both
        Console.WriteLine($"a | b  = {Bin(a | b)}");    // 11001111  OR  keeps bits set in either
        Console.WriteLine($"a ^ b  = {Bin(a ^ b)}");    // 11000101  XOR keeps bits that differ
        Console.WriteLine($"a >> 2 = {Bin(a >> 2)}");   // 00110010  shift right = divide by 4
        Console.WriteLine($"1 << 5 = {1 << 5}");        // 32        shift left  = multiply by 2^5

        // Flags: several yes/no values packed into one integer.
        const int Read = 1, Write = 2, Execute = 4;
        int permissions = Read | Write;
        Console.WriteLine((permissions & Write) != 0);     // True
        Console.WriteLine((permissions & Execute) != 0);   // False
    }
}
```

### Fixed-width integers overflow

```csharp
static class OverflowDemo
{
    public static void Run()
    {
        int max = int.MaxValue;                 // 2147483647, the largest 32-bit signed value
        Console.WriteLine(unchecked(max + 1));  // -2147483648: wraps around silently

        try { Console.WriteLine(checked(max + 1)); }
        catch (OverflowException) { Console.WriteLine("checked arithmetic throws instead"); }

        sbyte small = -37;
        Console.WriteLine(Convert.ToString((byte)small, 2));   // 11011011  two's complement of -37
    }
}
```

### Floating point is binary, so decimals are approximate

```csharp
static class FloatDemo
{
    public static void Run()
    {
        Console.WriteLine(0.1 + 0.2 == 0.3);          // False
        Console.WriteLine((0.1 + 0.2).ToString("R")); // 0.30000000000000004
        Console.WriteLine(Math.Abs(0.1 + 0.2 - 0.3) < 1e-9);   // True: compare with a tolerance
        Console.WriteLine(0.1m + 0.2m == 0.3m);       // True: decimal stores base-10 digits

        // The 32 bits of a float: 1 sign bit, 8 exponent bits, 23 mantissa bits.
        int bits = BitConverter.SingleToInt32Bits(0.1f);
        string text = Convert.ToString(bits, 2).PadLeft(32, '0');
        Console.WriteLine($"{text[..1]} {text[1..9]} {text[9..]}");   // 0 01111011 10011001100110011001101
    }
}
```

### Byte order (endianness)

```csharp
static class EndianDemo
{
    public static void Run()
    {
        byte[] bytes = BitConverter.GetBytes(0x12345678);
        Console.WriteLine(BitConverter.IsLittleEndian);       // True on x86/x64 and most ARM
        Console.WriteLine(BitConverter.ToString(bytes));      // 78-56-34-12: lowest byte stored first
    }
}
```

### Structs, classes and where the data lives

```csharp
struct PointStruct { public double X, Y; }   // value type: stored inline
class PointClass { public double X, Y; }     // reference type: object on the heap plus a reference

static class MemoryLayoutDemo
{
    public static void Run()
    {
        Console.WriteLine(Unsafe.SizeOf<PointStruct>());   // 16 bytes: two doubles, nothing else
        Console.WriteLine(Unsafe.SizeOf<PointClass>());    // 8 bytes: only the reference (on 64-bit)

        // One million structs are one contiguous block; one million objects are
        // a million separate allocations plus an array of references.
        long before = GC.GetTotalMemory(true);
        var structs = new PointStruct[1_000_000];
        long afterStructs = GC.GetTotalMemory(true);
        var objects = new PointClass[1_000_000];
        for (int i = 0; i < objects.Length; i++) objects[i] = new PointClass { X = i, Y = i };
        structs[0].X = structs[0].Y = 1;
        long afterObjects = GC.GetTotalMemory(true);

        Console.WriteLine($"struct array: {(afterStructs - before) / 1_000_000.0:F1} MB");        // about 16 MB
        Console.WriteLine($"class array:  {(afterObjects - afterStructs) / 1_000_000.0:F1} MB");  // about 40 MB
        GC.KeepAlive(structs);
        GC.KeepAlive(objects);
    }
}
```

### Span<T>: a view over memory without copying

```csharp
static class SpanDemo
{
    public static void Run()
    {
        int[] data = { 10, 20, 30, 40, 50, 60 };
        Span<int> middle = data.AsSpan(2, 3);   // views elements 30, 40, 50; no new array
        middle[0] = 99;
        Console.WriteLine(data[2]);             // 99: the span writes into the original

        ReadOnlySpan<char> line = "DN50;PN16;SUS304";
        int first = line.IndexOf(';');
        ReadOnlySpan<char> size = line[..first];          // no substring allocated
        Console.WriteLine(int.Parse(size[2..]) * 2);      // 100

        Span<byte> buffer = stackalloc byte[4];           // memory on the stack, freed on return
        buffer.Fill(0xFF);
        Console.WriteLine(BitConverter.ToString(buffer.ToArray()));   // FF-FF-FF-FF
    }
}
```

## Resources
- **Primary:** [Nand2Tetris](https://www.nand2tetris.org/) — Part I (build a computer from NAND gates)
- [Computer Systems: A Programmer's Perspective (CS:APP)](https://csapp.cs.cmu.edu/) — chapters 1–3, 6
- [.NET garbage collection fundamentals](https://learn.microsoft.com/en-us/dotnet/standard/garbage-collection/fundamentals)
- [What Every Programmer Should Know About Memory (Drepper)](https://people.freebsd.org/~lstewart/articles/cpumemory.pdf)

## Practice
- Nand2Tetris projects 1–6
- Write a C# program showing floating-point rounding error and fix it with `decimal`
- Measure row-major vs column-major traversal of a large 2D array and explain the gap
- Use `Span<T>` and `stackalloc` to remove allocations from a hot loop

## Checkpoint project
An **assembler or CPU emulator** for the Nand2Tetris Hack machine, written in C#.

# 19 — Programming languages and compilers (optional)

**Duration:** 4 weeks · **Prerequisites:** 04, 05, 07

**Exercises:** [14 exercises for this module](../exercises/19-languages-and-compilers.md)

## Goals
- Understand how source code is parsed, analysed and executed
- Be able to build small domain-specific languages and code-analysis tools

## Topics
1. Lexing and tokens
2. Grammars and recursive-descent parsing
3. Abstract syntax trees and the visitor pattern
4. Semantic analysis: scopes, types
5. Tree-walking interpreters
6. Bytecode and virtual machines
7. The .NET pipeline: Roslyn, IL, JIT
8. Other paradigms: a taste of functional programming (F#)

## Code examples

All of the C# below is one runnable project in [examples/19-languages-and-compilers](../examples/19-languages-and-compilers). Run it with `dotnet run` from that folder, then change it and run it again.

### Tokenizer: characters to tokens

```csharp
enum TokenKind { Number, Plus, Minus, Star, Slash, LeftParen, RightParen, End }

record Token(TokenKind Kind, string Text, int Position);

static class Tokenizer
{
    public static List<Token> Tokenize(string source)
    {
        var tokens = new List<Token>();
        int i = 0;
        while (i < source.Length)
        {
            char c = source[i];
            if (char.IsWhiteSpace(c)) { i++; continue; }

            if (char.IsDigit(c))
            {
                int start = i;
                while (i < source.Length && (char.IsDigit(source[i]) || source[i] == '.')) i++;
                tokens.Add(new Token(TokenKind.Number, source[start..i], start));
                continue;
            }

            TokenKind kind = c switch
            {
                '+' => TokenKind.Plus, '-' => TokenKind.Minus,
                '*' => TokenKind.Star, '/' => TokenKind.Slash,
                '(' => TokenKind.LeftParen, ')' => TokenKind.RightParen,
                _ => throw new FormatException($"Unexpected '{c}' at position {i}")
            };
            tokens.Add(new Token(kind, c.ToString(), i));
            i++;
        }
        tokens.Add(new Token(TokenKind.End, "", source.Length));
        return tokens;
    }
}

static class TokenizerDemo
{
    public static void Run() =>
        Console.WriteLine(string.Join(" ", Tokenizer.Tokenize("3 + 4.5 * (2 - 1)").Select(t => $"{t.Kind}:{t.Text}")));
}
```

### Syntax tree and recursive-descent parser

```csharp
// Grammar:   expression = term   { ("+" | "-") term }
//            term       = factor { ("*" | "/") factor }
//            factor     = NUMBER | "(" expression ")" | "-" factor
abstract record Expr;
record Num(double Value) : Expr;
record Negate(Expr Operand) : Expr;
record Binary(Expr Left, char Op, Expr Right) : Expr;

class Parser
{
    private readonly List<Token> _tokens;
    private int _position;

    public Parser(List<Token> tokens) => _tokens = tokens;

    private Token Current => _tokens[_position];
    private Token Take() => _tokens[_position++];

    public Expr ParseExpression()
    {
        Expr left = ParseTerm();
        while (Current.Kind is TokenKind.Plus or TokenKind.Minus)
            left = new Binary(left, Take().Text[0], ParseTerm());   // left-associative
        return left;
    }

    private Expr ParseTerm()   // one level deeper, so * and / bind tighter than + and -
    {
        Expr left = ParseFactor();
        while (Current.Kind is TokenKind.Star or TokenKind.Slash)
            left = new Binary(left, Take().Text[0], ParseFactor());
        return left;
    }

    private Expr ParseFactor()
    {
        Token token = Take();
        switch (token.Kind)
        {
            case TokenKind.Number:
                return new Num(double.Parse(token.Text, System.Globalization.CultureInfo.InvariantCulture));
            case TokenKind.Minus:
                return new Negate(ParseFactor());
            case TokenKind.LeftParen:
                Expr inner = ParseExpression();
                if (Take().Kind != TokenKind.RightParen) throw new FormatException("Expected ')'");
                return inner;
            default:
                throw new FormatException($"Unexpected '{token.Text}' at position {token.Position}");
        }
    }
}
```

### Tree-walking interpreter with pattern matching

```csharp
static class Interpreter
{
    public static double Evaluate(Expr expr) => expr switch
    {
        Num n => n.Value,
        Negate u => -Evaluate(u.Operand),
        Binary { Op: '+' } b => Evaluate(b.Left) + Evaluate(b.Right),
        Binary { Op: '-' } b => Evaluate(b.Left) - Evaluate(b.Right),
        Binary { Op: '*' } b => Evaluate(b.Left) * Evaluate(b.Right),
        Binary { Op: '/' } b => Evaluate(b.Left) / Evaluate(b.Right),
        _ => throw new InvalidOperationException("Unknown node")
    };

    public static string Show(Expr expr) => expr switch
    {
        Num n => n.Value.ToString(System.Globalization.CultureInfo.InvariantCulture),
        Negate u => $"(-{Show(u.Operand)})",
        Binary b => $"({Show(b.Left)} {b.Op} {Show(b.Right)})",
        _ => "?"
    };
}

static class ParserDemo
{
    public static void Run()
    {
        foreach (string source in new[] { "2 + 3 * 4", "(2 + 3) * 4", "10 - 4 - 3", "-(1 + 2) * 3" })
        {
            Expr tree = new Parser(Tokenizer.Tokenize(source)).ParseExpression();
            Console.WriteLine($"{source,-14} => {Interpreter.Show(tree),-20} = {Interpreter.Evaluate(tree)}");
        }
        // 14, 20, 3, -9
    }
}
```

### Compiling to bytecode for a stack machine

```csharp
record Instruction(string Op, double Operand = 0);

static class Compiler
{
    public static void Emit(Expr expr, List<Instruction> code)
    {
        switch (expr)
        {
            case Num n:
                code.Add(new Instruction("PUSH", n.Value));
                break;
            case Negate u:
                Emit(u.Operand, code);
                code.Add(new Instruction("NEG"));
                break;
            case Binary b:
                Emit(b.Left, code);    // operands first...
                Emit(b.Right, code);
                code.Add(new Instruction(b.Op switch { '+' => "ADD", '-' => "SUB", '*' => "MUL", _ => "DIV" }));   // ...then the operator
                break;
        }
    }
}

static class Machine
{
    public static double Run(List<Instruction> code)
    {
        var stack = new Stack<double>();
        foreach (Instruction instruction in code)
        {
            if (instruction.Op == "PUSH") { stack.Push(instruction.Operand); continue; }
            if (instruction.Op == "NEG") { stack.Push(-stack.Pop()); continue; }

            double right = stack.Pop(), left = stack.Pop();
            stack.Push(instruction.Op switch
            {
                "ADD" => left + right,
                "SUB" => left - right,
                "MUL" => left * right,
                _ => left / right
            });
        }
        return stack.Pop();
    }
}

static class CompilerDemo
{
    public static void Run()
    {
        Expr tree = new Parser(Tokenizer.Tokenize("2 + 3 * 4")).ParseExpression();
        var code = new List<Instruction>();
        Compiler.Emit(tree, code);

        Console.WriteLine(string.Join("  ", code.Select(i => i.Op == "PUSH" ? $"PUSH {i.Operand}" : i.Op)));
        // PUSH 2  PUSH 3  PUSH 4  MUL  ADD
        Console.WriteLine(Machine.Run(code));   // 14
    }
}
```

## Resources
- **Primary:** [Crafting Interpreters (free online)](https://craftinginterpreters.com/) — Part II in Java; port to C#
- [The .NET Compiler Platform SDK (Roslyn)](https://learn.microsoft.com/en-us/dotnet/csharp/roslyn-sdk/)
- [Nand2Tetris](https://www.nand2tetris.org/) — Part II (compiler and OS)
- [F# documentation](https://learn.microsoft.com/en-us/dotnet/fsharp/)

## Practice
- Arithmetic expression evaluator with correct precedence
- Port the Lox tree-walking interpreter to C#
- Write a Roslyn analyzer that flags one code smell
- Solve five Module 05 problems in F#

## Checkpoint project
An **interpreter for a small language** of your own design, or a DSL for a real task such as describing parametric components or report templates.

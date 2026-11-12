# 02 — Developer tools: shell, Git, debugging

**Duration:** 2 weeks · **Prerequisites:** 01

**Exercises:** [10 exercises for this module](../exercises/02-developer-tools.md)

## Goals
- Work comfortably in a terminal
- Use Git and GitHub for every project from here on
- Find bugs with a debugger instead of print statements

## Topics
1. Shell basics: navigation, pipes, redirection, environment variables (bash and PowerShell)
2. Git model: commits, branches, merge vs rebase, resolving conflicts
3. GitHub workflow: remotes, pull requests, issues, `.gitignore`, README
4. Debugging: breakpoints, stepping, watch, call stack, conditional breakpoints
5. NuGet packages and solution/project structure

## Code examples

All of the C# below is one runnable project in [examples/02-developer-tools](../examples/02-developer-tools). Run it with `dotnet run` from that folder, then change it and run it again.

### Debugging aids in code

```csharp
// Set a breakpoint on the line inside the loop, then use Step Over (F10),
// Step Into (F11) and the Watch window to follow "sum" and "i".
static class DebuggingAids
{
    static double Average(int[] values)
    {
        // An assertion documents an assumption and stops a Debug build when it is false.
        Debug.Assert(values.Length > 0, "values must not be empty");

        int sum = 0;
        for (int i = 0; i < values.Length; i++)
        {
            sum += values[i];   // <- breakpoint here; add a condition such as  i == 2
        }
        return (double)sum / values.Length;
    }

    public static void Run()
    {
        var timer = Stopwatch.StartNew();
        double avg = Average(new[] { 4, 8, 15, 16, 23, 42 });
        timer.Stop();

        Console.WriteLine($"average = {avg}");                              // 18
        Console.WriteLine($"took {timer.Elapsed.TotalMilliseconds:F3} ms");
        Debug.WriteLine("This goes to the debugger's Output window, not the console.");
    }
}
```

### Reading a stack trace

```csharp
// Read it from the top: the first line is where the exception was thrown,
// each following line is the caller of the one above.
static class ReadingStackTraces
{
    static int ParsePort(string text) => int.Parse(text);
    static int LoadConfig() => ParsePort("80a");

    public static void Run()
    {
        try
        {
            LoadConfig();
        }
        catch (FormatException ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);
        }
    }
}
```

### Shell basics (bash)

From [`snippets/01-shell.sh`](../examples/02-developer-tools/snippets/01-shell.sh).

```bash
pwd                         # where am I
ls -la                      # list files, including hidden ones
cd ~/projects && mkdir demo && cd demo

echo "hello" > notes.txt    # > overwrites a file
echo "world" >> notes.txt   # >> appends
cat notes.txt

# A pipe sends the output of one command into the next.
ls -la | wc -l                              # count directory entries
grep -r "TODO" --include="*.cs" . | wc -l   # count TODO comments in C# files
history | tail -n 20                        # the last 20 commands

# Variables and a loop
name="report"
for i in 1 2 3; do
  touch "${name}_${i}.txt"
done

export DOTNET_ENVIRONMENT=Development   # environment variable for child processes
echo "$DOTNET_ENVIRONMENT"
```

### A complete Git and GitHub workflow

From [`snippets/02-git.sh`](../examples/02-developer-tools/snippets/02-git.sh).

```bash
dotnet new console -o MyTool && cd MyTool
git init -b main
dotnet new gitignore              # ignore bin/, obj/ and editor files
git add .
git commit -m "Create console project"

# Work on a branch, never directly on main.
git switch -c feature/add-greeting
#   ...edit Program.cs...
git status                        # what changed?
git diff                          # exactly which lines?
git add Program.cs
git commit -m "Add greeting with user name"

# Publish and open a pull request on GitHub.
git remote add origin https://github.com/<you>/MyTool.git
git push -u origin feature/add-greeting

# After the pull request is merged:
git switch main
git pull
git branch -d feature/add-greeting

# Looking around and undoing
git log --oneline --graph --all   # history as a graph
git restore Program.cs            # throw away uncommitted changes to a file
git revert <commit>               # new commit that undoes an old one (safe on shared branches)
```

## Resources
- **Primary:** [MIT — The Missing Semester of Your CS Education](https://missing.csail.mit.edu/)
- [Pro Git (free book)](https://git-scm.com/book/en/v2) — chapters 1–3 and 6
- [Learn Git Branching](https://learngitbranching.js.org/) — interactive
- [GitHub Docs — Get started](https://docs.github.com/en/get-started)
- [Debugging in Visual Studio](https://learn.microsoft.com/en-us/visualstudio/debugger/debugger-feature-tour)

## Practice
- Finish the "Main" and "Remote" tracks of Learn Git Branching
- Create a merge conflict on purpose and resolve it
- Open a pull request against your own repository and merge it
- Fix a seeded bug in an old Module 01 program using only the debugger

## Checkpoint project
Move the Module 01 tracker into a clean GitHub repository: meaningful commit history, a feature branch merged by pull request, a README with build and usage instructions.

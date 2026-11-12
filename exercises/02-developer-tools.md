# Exercises 02 — Developer tools

Do the shell exercises in bash (WSL or Linux) and repeat at least two of them in PowerShell.

### 1. Find files ★
With one command each: count all `.cs` files under a directory tree; list the five largest files under it.

### 2. Log pipeline ★★
Given a log file whose lines begin with a date (`2026-01-15 10:32:01 ERROR ...`), print the number of `ERROR` lines per day, sorted by date, using only pipes of standard tools.

### 3. Project bootstrap script ★★
Write a script that takes a project name, creates a .NET console project, initialises Git, adds a .NET `.gitignore` (`dotnet new gitignore`), and makes the first commit.

### 4. Reading history ★
In a repository with at least five commits, use `git log --oneline --graph`, `git show` and `git diff` to answer: what changed in the third commit, and what differs between the first and the last?

### 5. Undoing things ★★
Practise each and write one sentence on what it did to the working tree, the index and the history: `git restore`, `git restore --staged`, `git commit --amend`, `git reset --soft HEAD~1`, `git revert`.

### 6. Merge conflict ★★
Create two branches that change the same line, merge them, and resolve the conflict by hand.

### 7. Rebase ★★
Create a feature branch with three commits, add a commit to `main`, then rebase the feature branch onto `main`. Explain how the history differs from a merge.

### 8. Bisect ★★★
Make ten commits to a small program and introduce a bug in one of the middle ones. Find the bad commit with `git bisect`.

### 9. Pull request ★★
On GitHub, open an issue, fix it on a branch, and open a pull request whose description contains `Closes #<issue number>`. Merge it and confirm the issue closed itself.

### 10. Debugger only ★★★
The program below should print `avg=-4.25 max=-1`. It has three bugs. Find and fix them using breakpoints, stepping and the watch window, without adding any `Console.WriteLine`.

```csharp
int[] data = { -5, -2, -9, -1 };
int sum = 0;
for (int i = 0; i <= data.Length; i++) sum += data[i];
double avg = sum / data.Length;
int max = 0;
foreach (var d in data) if (d > max) max = d;
Console.WriteLine($"avg={avg} max={max}");
```

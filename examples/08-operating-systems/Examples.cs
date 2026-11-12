// Run with: dotnet run
using System.Diagnostics;

ProcessInfoDemo.Run();
ChildProcessDemo.Run();
ThreadsDemo.Run();
RaceDemo.Run();
SignallingDemo.Run();
FileSystemDemo.Run();

// === What the OS knows about this process ===
static class ProcessInfoDemo
{
    public static void Run()
    {
        using var me = Process.GetCurrentProcess();
        Console.WriteLine($"process id:   {Environment.ProcessId}");
        Console.WriteLine($"name:         {me.ProcessName}");
        Console.WriteLine($"threads:      {me.Threads.Count}");
        Console.WriteLine($"working set:  {me.WorkingSet64 / 1_000_000} MB");   // physical memory in use
        Console.WriteLine($"cpu cores:    {Environment.ProcessorCount}");
        Console.WriteLine($"os:           {Environment.OSVersion}");
        Console.WriteLine($"PATH entries: {Environment.GetEnvironmentVariable("PATH")?.Split(Path.PathSeparator).Length}");
    }
}

// === Starting another process and reading its output ===
static class ChildProcessDemo
{
    public static void Run()
    {
        var info = new ProcessStartInfo("dotnet", "--list-runtimes")
        {
            RedirectStandardOutput = true,   // give us a pipe instead of the console
            UseShellExecute = false
        };

        using Process child = Process.Start(info)!;
        string output = child.StandardOutput.ReadToEnd();
        child.WaitForExit();

        Console.WriteLine($"child {child.Id} exited with code {child.ExitCode}");
        Console.WriteLine(output.Split('\n')[0]);
    }
}

// === Threads share memory inside one process ===
static class ThreadsDemo
{
    public static void Run()
    {
        var threads = new List<Thread>();
        for (int i = 1; i <= 3; i++)
        {
            int id = i;   // copy: each lambda needs its own variable
            var thread = new Thread(() =>
                Console.WriteLine($"worker {id} on managed thread {Environment.CurrentManagedThreadId}"));
            threads.Add(thread);
            thread.Start();
        }
        foreach (Thread t in threads) t.Join();   // wait for all of them
        Console.WriteLine("all workers finished (their order can change between runs)");
    }
}

// === A race condition and a lock ===
static class RaceDemo
{
    static int unsafeCounter, safeCounter;
    static readonly object Gate = new();

    public static void Run()
    {
        void Work()
        {
            for (int i = 0; i < 1_000_000; i++)
            {
                unsafeCounter++;                 // read, add, write: three steps that can interleave
                lock (Gate) { safeCounter++; }   // only one thread at a time inside the lock
            }
        }

        var a = new Thread(Work);
        var b = new Thread(Work);
        a.Start(); b.Start();
        a.Join(); b.Join();

        Console.WriteLine($"without lock: {unsafeCounter:N0}  (expected 2,000,000, usually less)");
        Console.WriteLine($"with lock:    {safeCounter:N0}");
    }
}

// === Semaphores: limiting and signalling ===
static class SignallingDemo
{
    public static void Run()
    {
        using var pumpsAvailable = new SemaphoreSlim(2);   // at most 2 holders at once
        int running = 0, peak = 0;
        var gate = new object();

        var threads = Enumerable.Range(1, 6).Select(_ => new Thread(() =>
        {
            pumpsAvailable.Wait();       // blocks while both permits are taken
            lock (gate) { running++; peak = Math.Max(peak, running); }
            Thread.Sleep(50);            // pretend to work
            lock (gate) { running--; }
            pumpsAvailable.Release();
        })).ToList();

        threads.ForEach(t => t.Start());
        threads.ForEach(t => t.Join());
        Console.WriteLine($"6 workers, never more than {peak} at once");   // 2
    }
}

// === Files, directories and metadata ===
static class FileSystemDemo
{
    public static void Run()
    {
        string folder = Path.Combine(Path.GetTempPath(), "os-demo");
        Directory.CreateDirectory(folder);
        string path = Path.Combine(folder, "data.bin");

        using (var stream = new FileStream(path, FileMode.Create, FileAccess.Write))
        {
            stream.Write(new byte[] { 1, 2, 3, 4, 5 });
            stream.Flush(flushToDisk: true);   // ask the OS to push its cache to the device
        }

        var info = new FileInfo(path);
        Console.WriteLine($"{info.Name}: {info.Length} bytes, modified {info.LastWriteTime:HH:mm:ss}");

        using (var stream = File.OpenRead(path))
        {
            stream.Seek(3, SeekOrigin.Begin);        // jump to byte offset 3
            Console.WriteLine(stream.ReadByte());    // 4
        }

        foreach (string file in Directory.EnumerateFiles(folder)) Console.WriteLine(Path.GetFileName(file));
        Directory.Delete(folder, recursive: true);
    }
}

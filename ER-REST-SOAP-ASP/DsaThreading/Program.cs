using System.Collections.Concurrent;
using System.Diagnostics;
using DsaThreading;

Console.WriteLine("Hello, World!");

ThreadingDemo();

static void ThreadingDemo()
{
    //How C# manages Threads (OS Threads), are an object, managed by the runtime behind scenes
    Console.WriteLine($"Main runs on thread #{Environment.CurrentManagedThreadId}");

    var workerThread = new Thread(() =>
    {
        Console.WriteLine($"Hello from Thread #{Environment.CurrentManagedThreadId}");
    });

    //Once we have a thread setup we have to manually start it.
    Console.WriteLine($"Before Start() call, isAlive = {workerThread.IsAlive}");
    workerThread.Start();
    Console.WriteLine($"During thread delegate code, isAlive = {workerThread.IsAlive}");
    workerThread.Join(); //Called from the Main function's thread.

    Console.WriteLine($"After Join() call, isAlive = {workerThread.IsAlive}");

    //Parallelism vs concurrency
    //Interleaving - Below even the runtime the actual OS scheduler
    //switches them on and off CPU threads really fast according to rules

    //Concurrency - tasks in progress (interleaved, even on one CPU core)
    //Parallelism - task executting at the same time (multiple cpu cores)

    //Threads give us concurrency, true parallelism depends on the hardware (and kernel).

    var threads = new List<Thread>();

    for (int i = 1; i <= 5; i++)
    {
        int id = i;

        var th = new Thread(() =>
        {
            Thread.Sleep(Random.Shared.Next(5, 40));
            Console.WriteLine($"Worker {id} finished on thread #{Environment.CurrentManagedThreadId}");
        });

        threads.Add(th);
        th.Start();
    }

    foreach (Thread thread in threads) thread.Join();

    //Thread Safe collection
    var counts = new ConcurrentDictionary<int, int>();

    var threadPool = new List<Thread>();

    for (int i = 1; i <= 8; i++)
    {
        int id = i;

        var th = new Thread(() =>
        {
            for (int k = 0; k < 1000; k++)
                counts.AddOrUpdate(id, 1, (_, prev) => prev + 1); //Third argument = a delegate to execute if the key already exists.
                                                                  //_ = C# discard - indicates the key parameter is intentionally ignored because the delegato wont use it.
        });

        threadPool.Add(th);
        th.Start();
    }

    foreach (var th in threadPool) th.Join();
    Console.WriteLine($"Recorded {counts.Values.Sum()} increments across {counts.Count} threads");

    //ThreadPool - we don't have to create or destroy threads, we just can borrow one

    var done = new ConcurrentQueue<int>();

    for (int i = 0; i < 5; i++)
    {
        int n = i;

        ThreadPool.QueueUserWorkItem(_ => done.Enqueue(n * n));
    }

    // Because we don't actually have the Threads themselves at our disposal - we'll
    //do like a crude await
    while (done.Count < 5) Thread.Sleep(5);

    Console.WriteLine($"Threadpool finished. {string.Join(", ", done.OrderBy(x => x))}");

    ParallelSum();

    static void ParallelSum()
    {
        int[] data = Enumerable.Range(1, 8000000).ToArray();

        var sw = Stopwatch.StartNew();

        long sequential = SumRange(data, 0, data.Length);
        sw.Stop();
        Console.WriteLine($"Sequential sum = {sequential}. {sw.ElapsedTicks} ticks, 1 thread");

        Task<long> half1 = Task.Run(() => SumRange(data, 0, data.Length / 2));
        Task<long> half2 = Task.Run(() => SumRange(data, data.Length / 2, data.Length));

        long total = half1.Result + half2.Result;
        Console.WriteLine($"Two task sum: {total}");

        long parallelTotal = 0;

        sw.Restart();

        Parallel.For(0, data.Length,
            () => 0L,
            (i, _, local) => local + data[i],
            local => Interlocked.Add(ref parallelTotal, local)
        );

        sw.Stop();
        Console.WriteLine($"Parallel sum =  {parallelTotal}. {sw.ElapsedTicks} ticks, multithread");
    }

    static long SumRange(int[] a, int start, int end)

    {
        long sum = 0;
        for (int i = start; i < end; i++)
        {
            sum += a[i];
        }

        return sum;
    }

    RaceDemo();

    static void RaceDemo()
    {
        var bank = new Bank();
        Parallel.For(0, 100000, _ => bank.DepositUnsafe(1));
        Console.WriteLine($"Unsafe balance = {bank.Balance} (expected 100000)");
    }

    SafeDemo();

    static void SafeDemo()
    {
        var bank = new Bank();
        Parallel.For(0, 100000, _ => bank.DepositSafe(1));
        Console.WriteLine($"SAFE balance = {bank.Balance} (expected 100000)");
    }
}
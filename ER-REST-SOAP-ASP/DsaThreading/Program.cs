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
    workerThread.Join(); //Called from the Main function's thread.

    Console.WriteLine($"After Join() call, isAlive = {workerThread.IsAlive}");

}
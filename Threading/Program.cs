Application();

void Application()
{
    DisplayThreadInfo();
    TwoNewLines();

    ContextSwitcher contextSwitcher = new ContextSwitcher();
    contextSwitcher.AlternateContext();
    TwoNewLines();

    SharedResources sharedResources = new SharedResources();
    sharedResources.ShareResources();
    TwoNewLines();

    LocalMemory localMemory = new LocalMemory();
    TwoNewLines();

    ThreadPooler threadPooler = new ThreadPooler();
    TwoNewLines();

    ThreadJoin threadJoin = new ThreadJoin();
    TwoNewLines();

    ThreadException threadException = new ThreadException();
    TwoNewLines();

    Console.WriteLine("Press the enter key to exit.");
    Console.Read();
}

void DisplayThreadInfo()
{
    var processorCount = Environment.ProcessorCount;
    Console.WriteLine("Processor count is: " + processorCount);

    ThreadPool.SetMaxThreads(processorCount, processorCount);

    int workerThreadCount = 0;
    int completionPortThreadCount = 0;
    ThreadPool.GetMinThreads(out workerThreadCount, out completionPortThreadCount);
    Console.WriteLine(
        "Worker thread count is: " + workerThreadCount + 
        ", completion port thread count is: " + completionPortThreadCount
        );
}

void TwoNewLines()
{
    Console.WriteLine("\n");
}

class ContextSwitcher
{
    public void AlternateContext()
    {
        Thread thread = new Thread(this.WriteUsingNewThread);
        thread.Name = "Z Threader";
        thread.Start();

        Thread.CurrentThread.Name = "Main Thread";
        for (int i = 0; i < 1000; i++)
            Console.Write(" A" + i + " ");
     
        thread.Join();
    }

    private void WriteUsingNewThread()
    {
        for (int i = 0; i < 1000; i++)
            Console.Write(" Z" + i + " ");
    }
}

class SharedResources
{
    private bool isCompleted;
    static readonly object lockCompleted = new object();

    public void ShareResources()
    {
        Thread thread = new Thread(this.HelloWorld);
        thread.Start();

        this.HelloWorld();

        thread.Join();
    }

    private void HelloWorld()
    {
        // Enforces a queue for this block of instructions by locking the object.
        // after the code reaches its final instruction, the object is unlocked.
        lock (lockCompleted)
        {
            if (isCompleted)
            {
            }
            else
            {
                Console.Write("Hello world one time.");
                isCompleted = true;
            }
        }
    }
}

class LocalMemory
{
    /// <summary>
    /// Purpose to show that local memory remains unchanged...
    /// </summary>
    public LocalMemory()
    {
        // Worker
        Thread thread = new Thread(this.PrintOneToTwentyFive);
        thread.Start();

        // Main
        this.PrintOneToTwentyFive();

        thread.Join();
    }

    private void PrintOneToTwentyFive()
    {
        for (int i = 0; i < 25; i++)
        {
            Console.Write(i + 1 + " ");
        }
    }
}

class ThreadPooler
{
    public ThreadPooler()
    {
        ThreadPool.QueueUserWorkItem(new WaitCallback(DisplayEmployeeInfo), new Employee { 
            Name = "Eric",
            CompanyName = ".NET"
        });
    }

    private void DisplayEmployeeInfo(object? state)
    {
        Console.WriteLine("Is this method part of the thread pool?: " + Thread.CurrentThread.IsThreadPoolThread);

        Employee? employee = state as Employee;

        Console.WriteLine($"Employee name is {employee.Name}, and their company is {employee.CompanyName}");
    }

    class Employee
    {
        public Employee()
        {
            Console.WriteLine("Is this employee part of the thread pool?: " + Thread.CurrentThread.IsThreadPoolThread);
        }
        public string? Name { get; set; }
        public string? CompanyName { get; set; }
    }
}

class ThreadJoin
{
    /// <summary>
    /// Useful for threading subprocesses.
    /// </summary>
    public ThreadJoin()
    {
        Thread thread = new Thread(DisplayHelloWorld);
        thread.Start();
        // Links the current contextual scope of the "bus".
        thread.Join();
        Console.WriteLine("Hello display was ran.");
    }

    private void DisplayHelloWorld()
    {
        Console.WriteLine("Hello world");
        Thread.Sleep(4000);
    }
}

class ThreadException
{
    public ThreadException()
    {
        this.Demo();
    }

    private void Demo()
    {
        Thread thread = new Thread(this.Error);
        thread.Start();
        thread.Join();
    }

    private void Error()
    {
        try
        {
            throw new Exception("Generic error message regarding exception handling with threads.");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}
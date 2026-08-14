async Task<int> Step1Async()
{
    Console.WriteLine("Step 1 starting...");
    await Task.Delay(2000);
    return 10;
}

async Task<int> Step2Async()
{
    Console.WriteLine("Step 2 starting...");
    await Task.Delay(2000);
    return 20;
}

// Sequential execution: Step2Async() will only start after Step1Async() has completed
var sw = System.Diagnostics.Stopwatch.StartNew();

var a = await Step1Async(); // fully waits here before moving on
var b = await Step2Async(); // only starts AFTER Step1 has completely finished

sw.Stop();
Console.WriteLine($"Total: {a + b}, took {sw.ElapsedMilliseconds}ms");

// Parallel execution: Step1Async() and Step2Async() will run concurrently
sw = System.Diagnostics.Stopwatch.StartNew();

Task<int> task1 = Step1Async(); // starts immediately, NOT awaited yet
Task<int> task2 = Step2Async(); // starts immediately too -- both running NOW

int[] results = await Task.WhenAll(task1, task2); // wait for BOTH to finish

sw.Stop();
Console.WriteLine($"Total: {results[0] + results[1]}, took {sw.ElapsedMilliseconds}ms");
## async/await Internals [.NET]

**What it is:** `async`/`await` lets a thread be freed while waiting on
I/O (database, network, disk) instead of sitting blocked and idle —
critical for servers handling many simultaneous requests.

**The core problem (Section 1):**
```csharp
// Blocking -- thread is stuck doing nothing for 2 seconds
public int GetTotal() => CallDatabase();

// Async -- thread is FREED during the wait, picked back up when ready
public async Task<int> GetTotalAsync() => await CallDatabaseAsync();
```
Blocking calls tie up a thread for the whole wait even though nothing
is actually being computed. Async frees the thread during I/O waits.

**My analogy:** Washing clothes (start machine, go take a bath, come
back and check) = concurrency — one "thread" (me) overlapping the wait
time of one task with another, NOT two things happening at the exact
same instant. True parallelism (`Task.WhenAll`) = two people, two
machines, genuinely simultaneous — covered in a later section.

**`Task<T>` is a "claim ticket" (Section 2), not the result itself:**
```csharp
Task<int> washTask = StartWashingAsync(); // the TICKET -- work already started
int result = await washTask;               // hands in the ticket, unwraps to the real value
```
- Calling an async method **starts it immediately** — you don't need
  to `await` it for the work to begin.
- NOT awaiting a `Task` ≠ blocking. It's the opposite: your code races
  ahead without waiting. Risk: you lose the ability to know if/when it
  finished or catch its errors ("fire and forget").
- **Type rule:** without `await`, the variable type must be `Task<T>`.
  With `await`, it must be the unwrapped `T` — never `Task<T>`:
```csharp
  Task<int> washTask = StartWashingAsync(); // ticket
  int result = await StartWashingAsync();    // unwrapped value
```

**Task states:** Running → Completed successfully / **Faulted**
(exception captured *inside* the Task, only re-thrown when awaited —
not at the moment it's actually thrown) / Canceled.

**Why `Task<T>` works with `await` — it's "awaitable":** implements
`GetAwaiter()` → `IsCompleted` + `GetResult()`. `await` is really:
"check if done; if not, resume here later; then call `GetResult()` to
unwrap the value."

**Gotcha — `Thread.Sleep()` is NOT the async-friendly wait.** Using it
inside an `async` method still blocks the thread — `async` on the
signature means nothing if the body blocks synchronously inside it:
```csharp
// WRONG -- still blocks the thread for 2 seconds
async Task<int> CountTheStudents()
{
    Thread.Sleep(2000);
    return 12;
}

// RIGHT -- frees the thread during the wait
async Task<int> CountTheStudents()
{
    await Task.Delay(2000);
    return 12;
}
```

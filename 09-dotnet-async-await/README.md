## async/await Internals [.NET]

**What it is:** `async`/`await` lets a thread be freed while waiting on
I/O (database, network, disk) instead of sitting blocked and idle —
critical for servers handling many simultaneous requests.

**The core problem :**
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

**`Task<T>` is a "claim ticket", not the result itself:**
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

### What async/await actually compiles into?

An `async` method compiles into a generated **state machine class**,
not "just a method." Confirmed this myself via `.GetType()` on an
un-awaited Task — it printed
`AsyncStateMachineBox<...g__CountTheStudents...>`, real proof of a
compiler-generated class.

**Simplified shape of what the compiler generates:**
```csharp
class CountTheStudentsStateMachine
{
    int state = 0; // "which step am I on?"
    TaskAwaiter delayAwaiter;

    public void MoveNext()
    {
        if (state == 0)
        {
            var delayTask = Task.Delay(2000);
            delayAwaiter = delayTask.GetAwaiter();
            if (!delayAwaiter.IsCompleted)
            {
                state = 1; // remember where to resume
                delayAwaiter.OnCompleted(MoveNext); // "call me back when done"
                return; // PAUSE -- thread is freed here
            }
        }
        if (state == 1)
        {
            delayAwaiter.GetResult();
            // ...set the final result on the outer Task
        }
    }
}
```
The `return;` at the pause point = "walking away to take a bath" —
control genuinely returns to the caller, thread is free. When the
delay finishes, **some** thread (same or different — not guaranteed)
calls `MoveNext()` again, picking up exactly at `state == 1` — not
restarting from the top.

**Threads/cores background (asked as a tangent, but foundational):**
- A **thread** = one sequence of instructions being run.
- A **core** = one "chef" that can genuinely execute only one
  instruction at a time.
- **Parallelism** = multiple cores, genuinely simultaneous work.
- **Concurrency** = one core, rapidly context-switching between many
  threads — an illusion of simultaneity built from fast switching.
- The **thread pool** is the "kitchen manager" handing free chefs to
  waiting work — this is exactly what a freed thread returns to after
  an `await` pause, and it's not reserved for the task it just left;
  it can pick up any pending work anywhere in the app.

---

### Sequential vs Parallel (hands-on, confirmed)

```csharp
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

// Sequential -- Step2 only starts after Step1 fully finishes
var sw = System.Diagnostics.Stopwatch.StartNew();
var a = await Step1Async();
var b = await Step2Async();
sw.Stop();
Console.WriteLine($"Total: {a + b}, took {sw.ElapsedMilliseconds}ms"); // ~4000ms

// Parallel -- both start immediately, wait for both together
sw = System.Diagnostics.Stopwatch.StartNew();
Task<int> task1 = Step1Async(); // starts now, not awaited yet
Task<int> task2 = Step2Async(); // starts now too -- both running concurrently
int[] results = await Task.WhenAll(task1, task2);
sw.Stop();
Console.WriteLine($"Total: {results[0] + results[1]}, took {sw.ElapsedMilliseconds}ms"); // ~2000ms
```

**Confirmed via my own run:** sequential took roughly double the
parallel version — because calling an async method starts it
immediately (Section 2), `task1`/`task2` are both genuinely running at
the same time the moment both lines execute; `Task.WhenAll` just waits
for whichever finishes last, so total time ≈ the *longer* delay, not
the *sum* of both.

**One-liner for interviews (partial, more tomorrow):** "Sequential
`await` waits for each operation to fully finish before starting the
next; starting tasks first and awaiting them together with
`Task.WhenAll` lets independent operations run concurrently, so total
time is bounded by the slowest one, not the sum."

---

### SynchronizationContext & ConfigureAwait(false)

**What it is:** some app types (WPF, WinForms, old ASP.NET) enforce
"only one specific thread — the UI thread — may touch the UI."
`SynchronizationContext` is what lets `await` automatically resume back
on that privileged context after the awaited work completes elsewhere.

```csharp
// WPF button click handler, runs on the UI thread
private async void Button_Click(object sender, EventArgs e)
{
    var data = await FetchDataAsync(); // resumes back on the UI thread automatically
    myLabel.Text = data; // safe -- still on the UI thread
}
```

**`ConfigureAwait(false)`** skips that "resume on the original context"
step — "any thread pool thread is fine to continue on":
```csharp
var data = await FetchDataAsync().ConfigureAwait(false);
```

**Why it matters in a library specifically:** library code doesn't know
or control what kind of app calls it. If a WPF app calls a library that
doesn't use `ConfigureAwait(false)`, every `await` inside tries to hop
back to the UI thread — wasted overhead, and a real **deadlock risk** if
the caller is also blocking synchronously (`.Result`) on that same UI
thread: the UI thread waits for the library, the library waits to get
back onto the UI thread — neither can proceed.

**Gotcha — it's about the privileged CONTEXT, not "the same thread that
called it."** My first pass at this got it slightly wrong: it's not
"the exact thread that invoked it" — it's specifically "the one thread
WPF designates as the UI thread." A console app has no such privileged
context, so any thread pool thread resuming the continuation is fine —
that's exactly why `Step1Async`/`Step2Async` above never needed
`ConfigureAwait(false)` to behave correctly.

**Rule of thumb:** library code with no reason to care which thread it
resumes on → use `ConfigureAwait(false)` defensively on every await.
Application-level code (e.g. a click handler) usually should let the
default behavior run, since it often genuinely needs the UI thread back.

---

### Full picture, tied together

1. **Section 1:** blocking calls waste threads during I/O waits.
2. **Section 2:** `Task<T>` is a "ticket" for work already in progress;
   `await` unwraps it into the real value.
3. **Section 3:** `async`/`await` compiles into a pause/resume state
   machine; `return` at a pause frees the thread.
4. **Section 4:** starting tasks first, awaiting together via
   `Task.WhenAll`, overlaps independent work — total time ≈ the
   slowest task, not the sum.
5. **Section 5:** `ConfigureAwait(false)` skips resuming on a
   privileged context (like the UI thread) — a defensive habit for
   library code, to avoid overhead and deadlocks.

**One-liner for interviews:** "`async`/`await` compiles into a
compiler-generated state machine that frees the thread during I/O
waits instead of blocking it. Starting independent tasks before
awaiting them (`Task.WhenAll`) lets them run concurrently, cutting
total time to roughly the slowest one. And I use `ConfigureAwait(false)`
in library code since it has no need to resume on a caller's privileged
thread context, avoiding both overhead and potential deadlocks."
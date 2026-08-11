## LINQ Deferred Execution [.NET]

**What it is:** A LINQ query (`.Where()`, etc.) is a "recipe," not a
result — it doesn't run when declared, only when **enumerated**
(`foreach`, `.ToList()`, `.Count()`, `.Any()`, etc.). This is called
**deferred execution**.

**My corrected example — deferred vs immediate:**
```csharp
List<Employee> employees = [
    new Employee { Id = 1, Name = "Sreejith", Age = 30 },
    new Employee { Id = 2, Name = "Lakshmi", Age = 28 },
    new Employee { Id = 3, Name = "Laiju", Age = 51 },
];

// --- Deferred ---
var deferredQuery = employees.Where(emp => emp.Age >= 50); // recipe only, not run
employees.Add(new Employee { Id = 4, Name = "Anu", Age = 85 }); // mutation BEFORE enumeration

foreach (var emp in deferredQuery) // runs NOW -- includes Anu
    Console.WriteLine(emp.Name);

// --- Immediate ---
var immediateQuery = employees.Where(emp => emp.Name.StartsWith("L")).ToList(); // runs NOW
employees.Add(new Employee { Id = 5, Name = "Lachu", Age = 15 }); // too late, already captured

foreach (var emp in immediateQuery) // does NOT include Lachu
    Console.WriteLine(emp.Name);
```

**Precise trigger:** it's **enumeration**, not "using the variable."
`Console.WriteLine(query)` doesn't run it; `foreach`/`.ToList()`/
`.Count()`/`.Any()`/`.First()` do.

**Gotcha — `FindAll()` is not LINQ.** It's a `List<T>` method (pre-dates
LINQ), always eager, always returns a real `List<T>` immediately.
`.Where()` is the LINQ method that's actually deferred:
```csharp
employees.FindAll(e => e.Name.StartsWith("L")); // always eager, no .ToList() needed
employees.Where(e => e.Name.StartsWith("L"));    // deferred until enumerated
```

**Where the query actually executes — this depends on the TYPE:**

| | `IEnumerable<T>` (e.g. `List<T>`) | `IQueryable<T>` (e.g. EF Core `DbSet<T>`) |
|---|---|---|
| Data location | Already in app memory | Lives in the database |
| `.Where()` builds | A delayed loop over memory | An expression tree → translated to SQL |
| Runs where | In the C# process (LINQ to Objects) | Inside the database engine (SQL) |
| Cost of re-enumerating | Wasted CPU, re-loops in memory | A **new network round-trip + SQL execution** each time |

```csharp
// IQueryable example (EF Core, Day 20+) -- same syntax, very different cost
var query = db.Orders.Where(o => o.Total > 100); // NO SQL sent yet
foreach (var o in query) { }  // SQL query #1 sent to the database
foreach (var o in query) { }  // SQL query #2 -- same query, sent AGAIN
```
EF Core's `.LogTo(Console.WriteLine)` makes this visible — two separate
`SELECT` statements print for the two loops above.

**When to use which:**
- **Default to immediate (`.ToList()`)** once you're done building the
  query and actually want the data. Guarantees exactly one execution
  (one DB round-trip, for `IQueryable`) and a consistent snapshot —
  re-using the result won't silently re-run anything or drift if the
  source changes mid-way.
- **Use deferred only while composing a query across multiple lines**,
  deliberately choosing one later point to run it:
```csharp
  var query = employees.Where(e => e.Age > 18); // not run yet
  if (onlyActive)
      query = query.Where(e => e.IsActive);       // still just refining the recipe
  var results = query.ToList();                    // runs once, fully composed
```

**Gotcha — "deferred = fresher data" is a trap, not a feature.**
Re-enumerating a deferred query doesn't safely "re-check" the source —
for `IQueryable`, it re-runs the full SQL query again (expensive); for
either type, enumerating the same query twice in one method can return
**inconsistent results** if the source changed in between, which is a
correctness bug, not just a performance one.

**One-liner for interviews:** "LINQ queries are deferred by default —
I call `.ToList()` once I'm ready to use the data, both to avoid
re-running the query (extra DB round-trips with `IQueryable`) and to
get a consistent snapshot instead of results that can silently shift
between enumerations."
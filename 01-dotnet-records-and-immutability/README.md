## Day 01 — Records & Immutability [.NET]

**What it is:** Records = value-based equality + immutable (`init`-only) properties.
Use `with` to get a modified copy without changing the original.

**Advantages:**
1. Immutable by default (`init`-only) — no accidental mutation
2. Value-based equality — compares data, not reference
3. `with` — non-destructive copy-and-modify
4. Free, readable `ToString()`
5. Built-in deconstruction (`var (id, name) = customer;`)

**Gotcha:** Immutability is shallow — a `List<T>` inside a record can still be mutated.

```csharp
public record Order(int Id, List<string> Items);

var order = new Order(1, new List<string> { "Keyboard" });

// This does NOT compile -- 'Items' is init-only, can't reassign the property
order.Items = new List<string> { "Mouse" }; // ERROR

// But this DOES compile and run just fine:
order.Items.Add("Mouse"); // no error at all!

Console.WriteLine(string.Join(", ", order.Items)); // Keyboard, Mouse
```

**One-liner for interviews:** "I use records for DTOs — immutability avoids
accidental mutation bugs, and equality compares by value, not reference."
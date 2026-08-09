## Day 05 — Nullable Reference Types [.NET]

**What it is:** NRT (C# 8) lets you declare, in the type itself, whether a
reference type is allowed to be `null`. The compiler then warns at
**compile time** if your code doesn't handle that possibility — it does
NOT change runtime behavior at all. `null` still works exactly like
before; NRT just stops the compiler from staying silent about the risk.

**The problem it solves:** A `NullReferenceException` used to be
possible on any reference type, with zero warning:
```csharp
Order order = GetOrder(); // what if this returns null?
Console.WriteLine(order.CustomerName.ToUpper()); // CRASH if order is null
```
Compiles clean, might pass testing, then crashes in production on an
edge case nobody caught.

**Enabling it:** `<Nullable>enable</Nullable>` in the `.csproj` (default
in new projects since .NET 6). Once enabled:
- `string` → "never null" (compiler warns if it might be)
- `string?` → "null is genuinely allowed"

**Gotcha 1 — NRT proves "not null," not "valid."** A non-nullable
`string` defaulted to `string.Empty` satisfies the compiler, but `""` is
still arguably invalid data — NRT can't catch that, only *your* code can.

**The fix — encapsulation, not just NRT:**
```csharp
public class Order
{
    public string CustomerName { get; }   // get-only -- can't be
    public string? Note { get; set; }     // changed from outside after
    public string Email { get; }          // construction
    public Order? PreviousOrder { get; set; }

    public Order(string customerName, string email)
    {
        if (string.IsNullOrWhiteSpace(customerName))
            throw new ArgumentException("Customer name is required.", nameof(customerName));
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required.", nameof(email));

        CustomerName = customerName;
        Email = email;
    }
}
```
Now `new Order("", "sree@x.com")` throws immediately — the class
protects its own invariants instead of trusting the caller to check.
Three layers, each catching something different:
- **NRT** → might be using a null value (compile time)
- **`required`** → forgot to set it at all (compile time)
- **Constructor validation** → set it to something technically non-null
  but still wrong, e.g. `""` (runtime, the only layer that knows what
  "valid" actually means for the business)

**The 4 everyday NRT operators:**
```csharp
// ?. -- null-conditional: safely access a member that might be null
Console.WriteLine($"Note: {currentOrder.Note?.ToUpper()}");

// ?? -- null-coalescing: fallback value if null
Console.WriteLine(currentOrder.Note ?? "No Notes provided");

// ??= -- null-coalescing assignment: assign only if currently null
currentOrder.Note ??= "N/A";

// ! -- null-forgiving: "trust me, not null here" -- suppresses the
// WARNING only, adds zero runtime protection
string lastName = currentOrder.LastName!;
Console.WriteLine(lastName.ToUpper());
```

**Gotcha 2 — `!` only does something if the compiler currently thinks
the value COULD be null.** Using `!` on an already-non-nullable property
is a silent no-op. And if you're wrong about it being safe, `!` doesn't
protect you at all — it just removes the warning; the crash still
happens at runtime, same as pre-NRT C#.

**One-liner for interviews:** "NRT catches a whole class of null bugs
at compile time, but it's a promise system, not a guarantee — I still
validate real business rules in constructors, and I'm careful with `!`
since it silences the compiler without adding any actual safety."
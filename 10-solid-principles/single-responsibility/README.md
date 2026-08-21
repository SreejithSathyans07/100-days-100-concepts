# Single Responsibility Principle (SRP)

## What it means
A class should have **only one reason to change** — meaning it should do just one job.

If you describe what a class does and find yourself saying "and," it's probably doing too much.

## Why it matters
- A change to one responsibility can accidentally break unrelated behavior.
- Classes with mixed responsibilities are harder to test in isolation.
- Reuse becomes difficult when unrelated logic is bundled together.

## Common trap: over-splitting
SRP does **not** mean "one method per class." It means "one reason to change per class."
A class can still manage its own core data/state — that's not a separate responsibility.

## Example

### ❌ Before (violates SRP)
```csharp
public class Order
{
    public List<string> Items { get; set; } = new List<string>();
    public decimal TotalAmount { get; set; }

    public void AddItem(string item, decimal price)
    {
        Items.Add(item);
        TotalAmount += price;
    }

    public void PrintInvoice()
    {
        Console.WriteLine("Invoice:");
        foreach (var item in Items)
            Console.WriteLine(item);
        Console.WriteLine($"Total: {TotalAmount}");
    }

    public void SendConfirmationEmail(string customerEmail)
    {
        Console.WriteLine($"Sending confirmation to {customerEmail}...");
    }
}
```

`Order` mixes three reasons to change:
1. **Order management** (adding items, tracking total) — its actual job
2. **Presentation** (printing an invoice) — changes if the report format changes
3. **Notification** (sending email) — changes if you switch to SMS, push notifications, etc.

### ✅ After (follows SRP)
```csharp
public class Order
{
    public List<string> Items { get; set; } = new List<string>();
    public decimal TotalAmount { get; set; }

    public void AddItem(string item, decimal price)
    {
        Items.Add(item);
        TotalAmount += price;
    }
}

public class Invoice
{
    public void PrintInvoice(Order order)
    {
        Console.WriteLine("Invoice:");
        foreach (var item in order.Items)
            Console.WriteLine(item);
        Console.WriteLine($"Total: {order.TotalAmount}");
    }
}

public class Email
{
    public void SendConfirmationEmail(string customerEmail)
    {
        Console.WriteLine($"Sending confirmation to {customerEmail}...");
    }
}
```

`AddItem` stays inside `Order` because managing its own items/total is Order's core
responsibility, not a separate concern. `PrintInvoice` and `SendConfirmationEmail`
move out because they're driven by unrelated reasons to change (presentation,
notification).

### Usage
```csharp
var order = new Order();
order.AddItem("Item1", 50.0m);

var invoice = new Invoice();
invoice.PrintInvoice(order);

var email = new Email();
email.SendConfirmationEmail("customer@example.com");
```

## In short
Split a class only when a piece of its behavior has a **different reason to change**
than the rest — not just because it's a separate method.
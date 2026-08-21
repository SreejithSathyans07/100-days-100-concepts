List<string> items = new List<string> { "Item1", "Item2", "Item3" };
Order order1 = new Order { Items = items, TotalAmount = 100.0m };
order1.AddItem("Item4", 50.0m);

Invoice invoice = new Invoice();
invoice.PrintInvoice(order1);

Email email = new Email();
email.SendConfirmationEmail("This is a sample email address");
public class Order
{
    public List<string> Items { get; set; } = new List<string>();
    public decimal TotalAmount { get; set; }

    public void AddItem(string item, decimal price)
    {
        this.Items.Add(item);
        this.TotalAmount += price;
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
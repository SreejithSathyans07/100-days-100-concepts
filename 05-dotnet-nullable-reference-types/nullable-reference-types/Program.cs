
Order orderFromEmp = new Order("John", "abc@gmail.com");

Order currentOrder = orderFromEmp.GetOrder();

// The null-conditional operator
Console.WriteLine($"Note for this order: {currentOrder.Note?.ToUpper()}");

// The null-coalescing operator ??
Console.WriteLine(currentOrder.Note ?? "No Notes provided");

// Null-coalescing assignment ??=
currentOrder.Note ??= "N/A";

Console.WriteLine(currentOrder.Note);

// The null-forgiving operator !
string lastName = currentOrder.LastName!;
Console.WriteLine(lastName.ToUpper());


public class Order
{
    public string CustomerName { get; } = string.Empty;
    public string? LastName { get; set;}
    public string? Note { get; set; }
    public string Email { get; } = string.Empty;
    public Order? PreviousOrder { get; set; }

    public Order GetOrder()
    {
        return this;
    }

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
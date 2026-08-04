

var customer1 = new Customer("John", "Doe", "john@example.com");
var customer2 = new Customer("Jane", "Smith", "jane@example.com");
var product = new Product("Laptop", 999.99m, 10);

bool areCustomersEqual = customer1 == customer2; // false
Console.WriteLine($"Are customers equal? {areCustomersEqual}");

// customer2.Email = "somenewemail@example.com" //not possible

var customer3 = customer1 with { Email = "thisisatest@example.com" };

// Printing the record
Console.WriteLine(customer3);

Employee emp1 = new Employee() {FirstName = "Sreejith", LastName = "Sathyan", Email = "someexample@gmail.com"};

// Printing the class
Console.WriteLine(emp1);

public record Customer(string FirstName, string LastName, string Email);
public record Product(string Name, decimal Price, int Quantity);
public class Employee()
{
    public string FirstName {get; set;} = string.Empty;
    public string LastName {get; set;} = string.Empty;
    public string Email {get; set;} = string.Empty;
}
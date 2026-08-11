
List<Employee> employees =
[
    new Employee { Id = 1, Name = "Sreejith", Age = 30 },
    new Employee { Id = 2, Name = "Lakshmi", Age = 28 },
    new Employee { Id = 3, Name = "Laiju", Age = 51 },
];
var queryOnly = employees.Where(emp => emp.Age >= 50);

employees.Add(new Employee { Id = 4, Name = "Anu", Age = 85 });

foreach(Employee emp in queryOnly)
{
    Console.WriteLine(emp.Name);
}

var immediateExecution = employees.Where(emp => emp.Name.StartsWith("L")).ToList();
employees.Add(new Employee { Id = 4, Name = "Lachu", Age = 15 });

foreach(Employee emp in immediateExecution)
{
    Console.WriteLine(emp.Name);
}

public class Employee
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
}
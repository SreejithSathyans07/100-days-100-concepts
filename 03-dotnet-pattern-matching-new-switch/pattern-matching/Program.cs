//traditional switch statement
string DayOfTheWeek(int day)
{
    switch (day)
    {
        case 1:
            return "Monday";
        case 2:
            return "Tuesday";
        case 3:
            return "Wednesday";
        default:
            return "Invalid day";
    }
}

Console.WriteLine(DayOfTheWeek(3)); // Output: "Wednesday"


string DayOfTheWeekNew(int day) => day switch
{
    1 => "Monday",
    2 => "Tuesday",
    3 => "Wednesday",
    _ => "Invalid day"
};

string DayOfTheWeekNew2(int day)
{
    return day switch
    {

        1 => "Monday",
        2 => "Tuesday",
        3 => "Wednesday",
        _ => "Invalid day"
    };
}
;

Console.WriteLine(DayOfTheWeekNew(3));
Console.WriteLine(DayOfTheWeekNew2(2));

// Matching with types
string TypeOfMyObject(object obj) => obj switch
{
    int i => "Obj is an integer",
    string s => "Obj is a string",
    _ => "Type not identified"
};

Console.WriteLine(TypeOfMyObject(2));
Console.WriteLine(TypeOfMyObject("Sreejith"));
Console.WriteLine(TypeOfMyObject(2.5));

// Matching with properties

var user1 = new UserDto("Sreejith", "IT");
var user2 = new UserDto("Lakshmi", "Health");

string CheckFreeTransportEligibility(UserDto user) => user switch
{
    UserDto { Department: "Health" } u => $"{u.EmpName} is eligible for free transport",
    UserDto u => $"{u.EmpName} belongs to {u.Department}. So, the employee is not eligible for free transportation.",
    null => "User is null",
};

Console.WriteLine(CheckFreeTransportEligibility(user1));
Console.WriteLine(CheckFreeTransportEligibility(user2));
Console.WriteLine(CheckFreeTransportEligibility(null));


// Matching with relational patterns and combinations

var user3 = new UserDtoNew("Sreejith", "IT", 30);
var user4 = new UserDtoNew("Manu", "Finance", 12);
var user5 = new UserDtoNew("Abhi", "Teaching", 65);

string FindUserAge(UserDtoNew user) => user switch
{
    UserDtoNew { Age: >= 0 and < 18 } u => $"{u.EmpName} is a teenager",
    UserDtoNew { Age: > 18 and < 60 } u => $"{u.EmpName} is an adult",
    UserDtoNew { Age: >= 60 } u => $"{u.EmpName} is a senior citizen"
};

Console.WriteLine(FindUserAge(user3));
Console.WriteLine(FindUserAge(user4));
Console.WriteLine(FindUserAge(user5));
public record UserDto(string EmpName, string Department);
public record UserDtoNew(string EmpName, string Department, int Age);




Task<int> StudentCountTask = CountTheStudents();
Console.WriteLine(StudentCountTask.GetType());

int StudentsCountFromTask = await StudentCountTask;
Console.WriteLine(StudentsCountFromTask);

int StudentsCount = await CountTheStudents();
Console.WriteLine(StudentsCount);

async Task<int> CountTheStudents()
{
    Console.WriteLine("** counting the students");
    await Task.Delay(2000);
    return 12;
}
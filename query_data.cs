using Microsoft.EntityFrameworkCore;
using ConsoleApp1.Models;

class QeruryData
{

    public static void RetrieveAndPrintStudents(
        TrainingCenterDbContext context)
    {
        Console.WriteLine("Example 1 - Retrieve Students");
        Console.WriteLine("=============================");
        Console.WriteLine();

        // Build query first
        var query = context.Students
            .Where(s => s.Status == "Active")
            .OrderBy(s => s.StudentId);

        // Preview SQL
        Console.WriteLine("Preview SQL using ToQueryString():");
        Console.WriteLine("----------------------------------");
        Console.WriteLine(query.ToQueryString());
        Console.WriteLine();

        // Execute query
        var students = query.ToList();

        Console.WriteLine(
            $"Rows Returned: {students.Count}");

        Console.WriteLine();
        Console.WriteLine(new string('=', 70));
        Console.WriteLine();
    }

    public static void GetActiveStudentsCount(
        TrainingCenterDbContext context)
    {
        Console.WriteLine("Example 2 - Count Comparison");
        Console.WriteLine("============================");
        Console.WriteLine();

        // Build query first
        var query = context.Students
            .Where(s => s.Status == "Active");

        // Preview SQL before Count()
        Console.WriteLine("Preview SQL using ToQueryString():");
        Console.WriteLine("----------------------------------");
        Console.WriteLine(query.ToQueryString());
        Console.WriteLine();

        Console.WriteLine(
            "Now executing Count()...");
        Console.WriteLine(
            "Watch logging output above / below.");
        Console.WriteLine();

        // Actual execution
        int total = query.Count();

        Console.WriteLine(
            $"Total Active Students: {total}");
        foreach (var student in query)
        {
            Console.WriteLine($"{student.FirstName} {student.LastName}");
        }
        Console.WriteLine();
        Console.WriteLine(
            "Important Note:");
        Console.WriteLine(
            "ToQueryString() previewed SELECT rows query.");
        Console.WriteLine(
            "But logging shows final executed COUNT(*) query.");

        Console.WriteLine();
        Console.WriteLine(new string('=', 70));
        Console.WriteLine();
    }

    /// <summary>
    /// Retrieves only active students using Where()
    /// </summary>
    public static void GetActiveStudents(TrainingCenterDbContext context)
    {
        // Build query (no execution yet)
        var query = context.Students
            .Where(s => s.Status == "Active")
            .OrderBy(s => s.StudentId);

        // Show generated SQL
        PreviewSQLUsingToQueryString(query.ToQueryString());


        // Execute query
        var students = query.ToList();

        // Print results
        Console.WriteLine("\nActive Students:");
        Console.WriteLine("----------------");

        foreach (var student in students)
        {
            Console.WriteLine($"{student.StudentId} - {student.FirstName} {student.LastName}");
        }

        Console.WriteLine();
        Console.WriteLine($"Total Active Students: {students.Count}");
    }

    /// <summary>
    /// First() returns the first matching row.
    /// Use it when at least one row is expected.
    /// </summary>
    public static void Example_First(TrainingCenterDbContext context)
    {
        Console.WriteLine("\nExample 1 - First()");
        Console.WriteLine("-------------------");

        // Build query first (no execution yet)
        var query = context.Students
            .Where(s => s.Status == "Active")
            .OrderBy(s => s.StudentId);

        // Preview query shape
        PreviewSQLUsingToQueryString(query.ToQueryString());

        // Execute query
        // Runtime logging will show the actual executed SQL.
        var student = query.First();

        Console.WriteLine("\nFirst Active Student:");
        Console.WriteLine($"{student.StudentId} - {student.FirstName} {student.LastName}");
    }

    /// <summary>
    /// FirstOrDefault() returns the first matching row,
    /// or null if no row exists.
    /// </summary>
    public static void Example_FirstOrDefault(TrainingCenterDbContext context)
    {
        Console.WriteLine("\nExample 2 - FirstOrDefault()");
        Console.WriteLine("----------------------------");

        // Build query first (no execution yet)
        var query = context.Students
            .Where(s => s.Email == "notfound@student.com");

        // Preview query shape
        PreviewSQLUsingToQueryString(query.ToQueryString());

        // Execute query
        // Runtime logging will show the actual executed SQL.
        var student = query.FirstOrDefault();

        if (student == null)
        {
            Console.WriteLine("\nNo student found.");
        }
        else
        {
            Console.WriteLine("\nStudent Found:");
            Console.WriteLine($"{student.StudentId} - {student.FirstName} {student.LastName}");
        }
    }

    /// <summary>
    /// Single() expects exactly one matching row.
    /// Use it when the data must be unique.
    /// </summary>
    public static void Example_Single(TrainingCenterDbContext context)
    {
        Console.WriteLine("\nExample 3 - Single()");
        Console.WriteLine("--------------------");

        // Build query first (no execution yet)
        var query = context.Courses
            .Where(c => c.Code == "EF-101");

        // Preview query shape
        PreviewSQLUsingToQueryString(query.ToQueryString());

        // Execute query
        // Runtime logging will show the actual executed SQL.
        var course = query.Single();

        Console.WriteLine("\nCourse Found:");
        Console.WriteLine($"{course.CourseId} - {course.Code} - {course.Title}");
    }

    /// <summary>
    /// SingleOrDefault() expects zero or one matching row.
    /// Returns null if none exists, but throws if duplicates exist.
    /// </summary>
    public static void Example_SingleOrDefault(TrainingCenterDbContext context)
    {
        Console.WriteLine("\nExample 4 - SingleOrDefault()");
        Console.WriteLine("-----------------------------");

        // Build query first (no execution yet)
        var query = context.Courses
            .Where(c => c.Code == "UNKNOWN-999");

        // Preview query shape
        PreviewSQLUsingToQueryString(query.ToQueryString());

        // Execute query
        // Runtime logging will show the actual executed SQL.
        var course = query.SingleOrDefault();

        if (course == null)
        {
            Console.WriteLine("\nNo course found.");
        }
        else
        {
            Console.WriteLine("\nCourse Found:");
            Console.WriteLine($"{course.CourseId} - {course.Code} - {course.Title}");
        }
    }

    /// <summary>
    /// Retrieves student by Primary Key using Find().
    /// Best method for direct PK lookup.
    /// May return tracked entity without executing SQL again.
    /// </summary>
    public static void GetStudentByIdUsingFind(TrainingCenterDbContext context)
    {
        Console.WriteLine("Using Find()");
        Console.WriteLine("------------");

        // Find() does not support ToQueryString().
        // Runtime logging will show actual SQL only if query is sent to database.
        var student = context.Students.Find(1);

        PrintStudent(student);
    }

    /// <summary>
    /// Retrieves student by Primary Key using FirstOrDefault().
    /// Useful when filtering with conditions.
    /// </summary>
    public static void GetStudentByIdUsingFirstOrDefault(TrainingCenterDbContext context)
    {
        Console.WriteLine("Using FirstOrDefault()");
        Console.WriteLine("----------------------");

        // Build query first
        var query =
            context.Students
                   .Where(s => s.StudentId == 1);

        // Preview SQL query shape
        PreviewSQLUsingToQueryString(query.ToQueryString());

        // Execute query
        // ToQueryString previews query shape,
        // runtime logging shows actual executed SQL.
        var student = query.FirstOrDefault();

        PrintStudent(student);
    }

    /// <summary>
    /// Retrieves only student names using projection.
    /// </summary>
    public static void GetStudentNames(TrainingCenterDbContext context)
    {
        Console.WriteLine("Projection Example Using Select()");
        Console.WriteLine("---------------------------------");
        Console.WriteLine();

        // Build query first (no execution yet)
        var query =
            context.Students
                   .Select(s => new
                   {
                       s.FirstName,
                       s.LastName
                   });

        // Preview SQL before execution
        PreviewSQLUsingToQueryString(query.ToQueryString());

        // Execute query
        var students = query.ToList();

        // Print results
        Console.WriteLine("\n\nStudent Names:");
        Console.WriteLine("--------------");

        foreach (var student in students)
        {
            Console.WriteLine($"{student.FirstName} {student.LastName}");
        }

        Console.WriteLine();
        Console.WriteLine($"\nTotal Students: {students.Count}");
        Console.WriteLine();
    }






    static void PreviewSQLUsingToQueryString(string SQLString)
    {
        Console.WriteLine("\nPreview SQL using ToQueryString():");
        Console.WriteLine("----------------------------------");
        Console.WriteLine(SQLString);
        Console.WriteLine();

    }
    static void PrintStudent(dynamic? student)
    {
        if (student != null)
        {
            Console.WriteLine("\n\nStudent Found:");
            Console.WriteLine(
                $"{student.StudentId} - {student.FirstName} {student.LastName}");
        }
        else
        {
            Console.WriteLine("Student not found.");
        }

        Console.WriteLine();
    }
}
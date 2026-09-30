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

    /// <summary>
    /// Demonstrates combining Where(), Select(), and OrderByDescending().
    /// </summary>
    public static void GetFilteredSortedStudents(TrainingCenterDbContext context)
    {
        Console.WriteLine("Filtered Projection With Sorting");
        Console.WriteLine("--------------------------------");
        Console.WriteLine();

        // Build query first
        var query =
            context.Students
                   .Where(s => s.Status == "Active") // Filter
                   .Select(s => new                  // Projection
                   {
                       s.StudentId,
                       FullName = s.FirstName + " " + s.LastName
                   })
                   .OrderByDescending(s => s.StudentId); // Sorting

        // Preview SQL before execution
        PreviewSQLUsingToQueryString(query.ToQueryString());

        // Execute query
        var students = query.ToList();

        // Print results
        Console.WriteLine("\n\nFiltered Students:");
        Console.WriteLine("------------------");

        foreach (var student in students)
        {
            Console.WriteLine($"{student.StudentId} - {student.FullName}");
        }

        Console.WriteLine();
        Console.WriteLine($"Total Students: {students.Count}");
        Console.WriteLine();
    }

    /// <summary>
    /// Demonstrates Any() and All().
    /// </summary>
    public static void CheckDataWithAnyAndAll(TrainingCenterDbContext context)
    {
        Console.WriteLine("Any() and All() Example");
        Console.WriteLine("-----------------------");
        Console.WriteLine();

        // --------------------------------------------------
        // Any() Example
        // --------------------------------------------------

        // Build query first
        var activeStudentsQuery =
            context.Students
                   .Where(s => s.Status == "Active");

        // Preview SQL query shape
        PreviewSQLUsingToQueryString(activeStudentsQuery.ToQueryString());

        // Execute query
        // ToQueryString previews query shape,
        // runtime logging shows actual executed SQL for Any().
        bool hasActiveStudents =
            activeStudentsQuery.Any();

        Console.WriteLine($"Has Active Students: {hasActiveStudents}");
        Console.WriteLine();

        // --------------------------------------------------
        // All() Example
        // --------------------------------------------------

        // Build query first
        var coursesQuery =
            context.Courses;

        // Preview SQL query shape
        PreviewSQLUsingToQueryString(coursesQuery.ToQueryString());

        // Execute query
        // ToQueryString previews query shape,
        // runtime logging shows actual executed SQL for All().
        bool allCoursesValid =
            coursesQuery.All(c => c.Price > 0);

        Console.WriteLine($"All Courses Price > 0: {allCoursesValid}");
        Console.WriteLine();
    }

    /// <summary>
    /// Compares bad vs good COUNT approach.
    /// </summary>
    public static void CompareCount(TrainingCenterDbContext context)
    {
        Console.WriteLine("COUNT EXAMPLE");
        Console.WriteLine();

        Console.WriteLine("BAD WAY:");
        Console.WriteLine();

        // Build query first
        var badQuery =
            context.Students;

        // Preview SQL query shape
        PreviewSQLUsingToQueryString(badQuery.ToQueryString());

        // Execute query and load all rows into memory
        var students = badQuery.ToList();

        // Count happens in memory after data is already loaded
        int badCount =
            students.Count(s => s.Status == "Active");

        Console.WriteLine($"Bad Count (calculated in memory): {badCount}");
        Console.WriteLine();

        Console.WriteLine("GOOD WAY:");
        Console.WriteLine();

        // Build query first
        var goodQuery =
            context.Students
                   .Where(s => s.Status == "Active");

        // Preview SQL query shape
        PreviewSQLUsingToQueryString(goodQuery.ToQueryString());

        // Execute COUNT in the database
        // ToQueryString previews query shape,
        // runtime logging shows actual executed SQL for Count().
        int goodCount =
            goodQuery.Count();

        Console.WriteLine($"Good Count (calculated in database): {goodCount}");
        Console.WriteLine();
    }

    /// <summary>
    /// Compares bad vs good AVERAGE approach.
    /// </summary>
    public static void CompareAverage(TrainingCenterDbContext context)
    {
        Console.WriteLine("AVERAGE EXAMPLE");
        Console.WriteLine();

        Console.WriteLine("BAD WAY:");
        Console.WriteLine();

        // Build query first
        var badQuery =
            context.Enrollments;

        // Preview SQL query shape
        PreviewSQLUsingToQueryString(badQuery.ToQueryString());

        // Execute query and load all rows into memory
        var enrollments = badQuery.ToList();

        // Average happens in memory after data is already loaded
        decimal badAverage =
            enrollments.Average(e => e.ProgressPercent);

        Console.WriteLine($"Bad Average (calculated in memory): {badAverage}");
        Console.WriteLine();

        Console.WriteLine("GOOD WAY:");
        Console.WriteLine();

        // Build query first
        var goodQuery =
            context.Enrollments
                   .Select(e => e.ProgressPercent);

        // Preview SQL query shape
        PreviewSQLUsingToQueryString(goodQuery.ToQueryString());

        // Execute AVERAGE in the database
        // ToQueryString previews query shape,
        // runtime logging shows actual executed SQL for Average().
        decimal goodAverage =
            goodQuery.Average();

        Console.WriteLine($"Good Average (calculated in database): {goodAverage}");
        Console.WriteLine();
    }

    /// <summary>
    /// Compares bad vs good SUM approach.
    /// </summary>
    public static void CompareSum(TrainingCenterDbContext context)
    {
        Console.WriteLine("SUM EXAMPLE");
        Console.WriteLine();

        Console.WriteLine("BAD WAY:");
        Console.WriteLine();

        // Build query first
        var badQuery =
            context.Courses;

        // Preview SQL query shape
        PreviewSQLUsingToQueryString(badQuery.ToQueryString());

        // Execute query and load all rows into memory
        var courses = badQuery.ToList();

        // Sum happens in memory after data is already loaded
        int badSum =
            courses.Sum(c => c.DurationHours);

        Console.WriteLine($"Bad Sum (calculated in memory): {badSum}");
        Console.WriteLine();

        Console.WriteLine("GOOD WAY:");
        Console.WriteLine();

        // Build query first
        var goodQuery =
            context.Courses
                   .Select(c => c.DurationHours);

        // Preview SQL query shape
        PreviewSQLUsingToQueryString(goodQuery.ToQueryString());

        // Execute SUM in the database
        // ToQueryString previews query shape,
        // runtime logging shows actual executed SQL for Sum().
        int goodSum =
            goodQuery.Sum();

        Console.WriteLine($"Good Sum (calculated in database): {goodSum}");
        Console.WriteLine();
    }

    /// <summary>
    /// Demonstrates Min() and Max() using TrainingCenterDB.
    /// </summary>
    public static void ShowMinMax(TrainingCenterDbContext context)
    {
        Console.WriteLine("Min() and Max() Example");
        Console.WriteLine("-----------------------");
        Console.WriteLine();

        // --------------------------------------------------
        // Lowest Course Price
        // --------------------------------------------------

        // Build query first
        var coursePricesQuery =
            context.Courses
                   .Select(c => c.Price);

        // Preview SQL query shape
        PreviewSQLUsingToQueryString(coursePricesQuery.ToQueryString());

        // Execute query
        // ToQueryString previews query shape,
        // runtime logging shows actual executed SQL for Min().
        decimal lowestPrice =
            coursePricesQuery.Min();

        // Execute query
        // ToQueryString previews query shape,
        // runtime logging shows actual executed SQL for Max().
        decimal highestPrice =
            coursePricesQuery.Max();

        // --------------------------------------------------
        // Earliest Registration Date
        // --------------------------------------------------

        // Build query first
        var registrationDatesQuery =
            context.Students
                   .Select(s => s.RegisteredAt);

        // Preview SQL query shape
        PreviewSQLUsingToQueryString(registrationDatesQuery.ToQueryString());

        // Execute query
        // ToQueryString previews query shape,
        // runtime logging shows actual executed SQL for Min().
        DateTime earliestRegistration =
            registrationDatesQuery.Min();

        // Print readable output
        Console.WriteLine($"Lowest Course Price     : {lowestPrice}");
        Console.WriteLine($"Highest Course Price    : {highestPrice}");
        Console.WriteLine($"Earliest Registration   : {earliestRegistration:d}");
        Console.WriteLine();
    }

    /// <summary>
    /// Shows unique student statuses using Distinct().
    /// </summary>
    public static void ShowDistinctStudentStatuses(TrainingCenterDbContext context)
    {
        Console.WriteLine("Unique Student Statuses");
        Console.WriteLine("-----------------------");

        // Build query first
        var query =
            context.Students
                   .Select(s => s.Status)
                   .Distinct();

        // Preview SQL before execution
        PreviewSQLUsingToQueryString(query.ToQueryString());



        // Execute query
        var statuses = query.ToList();

        Console.WriteLine();
        // Print readable output
        foreach (var status in statuses)
        {
            Console.WriteLine(status);
        }
    }

    /// <summary>
    /// Shows number of students grouped by status.
    /// </summary>
    public static void ShowStudentsGroupByStatusReport(TrainingCenterDbContext context)
    {
        Console.WriteLine("Students Per Status");
        Console.WriteLine("-------------------");

        // Build query first
        var query =
            context.Students
                   .GroupBy(s => s.Status)
                   .Select(g => new
                   {
                       Status = g.Key,
                       TotalStudents = g.Count()
                   })
                   .OrderBy(x => x.Status);

        // Preview SQL before execution
        PreviewSQLUsingToQueryString(query.ToQueryString());

        // Execute query
        // ToQueryString previews query shape,
        // runtime logging shows actual executed SQL for Count().
        var report = query.ToList();

        Console.WriteLine();
        // Print readable output
        foreach (var row in report)
        {
            Console.WriteLine($"{row.Status} : {row.TotalStudents}");
        }
    }

    /// <summary>
    /// Shows statuses having more than 2 students.
    /// </summary>
    public static void ShowStudentsPerStatusHaving(TrainingCenterDbContext context)
    {
        Console.WriteLine("Statuses With More Than 2 Students");
        Console.WriteLine("----------------------------------");

        // Build query first
        var query =
            context.Students
                   .GroupBy(s => s.Status)
                   .Where(g => g.Count() > 6)
                   .Select(g => new
                   {
                       Status = g.Key,
                       TotalStudents = g.Count()
                   })
                   .OrderBy(x => x.Status);

        // Preview SQL before execution
        PreviewSQLUsingToQueryString(
            query.ToQueryString());

        // Execute query
        var report = query.ToList();

        Console.WriteLine();

        foreach (var row in report)
        {
            Console.WriteLine(
                $"{row.Status} : {row.TotalStudents}");
        }
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
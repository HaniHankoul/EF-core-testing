using Microsoft.EntityFrameworkCore;
using ConsoleApp1.Models;

class AdvancedQueryData
{

    /// <summary>
    /// Demonstrates the bad N+1 approach by loading students first,
    /// then running one additional count query per student.
    /// </summary>
    public static void ShowBadNPlusOneApproach(TrainingCenterDbContext context)
    {
        Console.WriteLine("BAD APPROACH - N+1 Problem");
        Console.WriteLine("--------------------------");
        Console.WriteLine();

        // Build query first
        var studentsQuery =
            context.Students;

        // Preview SQL before execution
        PreviewSQLUsingToQueryString(studentsQuery.ToQueryString());

        // Execute first query: load all students
        var students = studentsQuery.ToList();

        Console.WriteLine();

        foreach (var student in students)
        {
            // Build query first for each student
            var enrollmentsQuery =
                context.Enrollments
                       .Where(e => e.StudentId == student.StudentId);

            // Preview SQL query shape
            PreviewSQLUsingToQueryString(enrollmentsQuery.ToQueryString());

            // Execute Count() for each student
            // ToQueryString previews query shape,
            // runtime logging shows actual executed SQL for Count().
            int enrollmentsCount =
                enrollmentsQuery.Count();

            Console.WriteLine(
                $"{student.FirstName} {student.LastName} - Enrollments: {enrollmentsCount}");
        }

        Console.WriteLine();
        Console.WriteLine("Problem: One query for students + one query per student.");
        Console.WriteLine();
    }

    private static void PreviewSQLUsingToQueryString(object value)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Demonstrates the good approach by loading students with their enrollments using Include().
    /// </summary>
    public static void ShowGoodIncludeApproach(TrainingCenterDbContext context)
    {
        Console.WriteLine("GOOD APPROACH - Include()");
        Console.WriteLine("-------------------------");
        Console.WriteLine();

        // Build query first
        var query =
            context.Students
                   .Include(s => s.Enrollments);

        // Preview SQL before execution
        PreviewSQLUsingToQueryString(query.ToQueryString());

        // Execute query
        var studentsWithEnrollments = query.ToList();

        Console.WriteLine();
        foreach (var student in studentsWithEnrollments)
        {
            Console.WriteLine(
                $"{student.FirstName} {student.LastName} - Enrollments: {student.Enrollments.Count}");
        }

        Console.WriteLine();
        Console.WriteLine("Result: Related enrollments are loaded with the students.");
        Console.WriteLine();
    }

    /// <summary>
    /// Demonstrates the best approach using projection to retrieve only required data.
    /// </summary>
    public static void ShowBestProjectionApproach(TrainingCenterDbContext context)
    {
        Console.WriteLine("BEST APPROACH - Projection");
        Console.WriteLine("--------------------------");
        Console.WriteLine();

        // Build query first
        var projectionQuery =
            context.Students
                   .Select(s => new
                   {
                       s.FirstName,
                       s.LastName,
                       EnrollmentsCount = s.Enrollments.Count()
                   });

        // Preview SQL before execution
        PreviewSQLUsingToQueryString(projectionQuery.ToQueryString());

        // Execute query
        // ToQueryString previews query shape,
        // runtime logging shows actual executed SQL for Count().
        var result = projectionQuery.ToList();

        Console.WriteLine();
        foreach (var student in result)
        {
            Console.WriteLine(
                $"{student.FirstName} {student.LastName} - {student.EnrollmentsCount}");
        }

        Console.WriteLine();
        Console.WriteLine("Result: One query, minimal data, best performance.");
        Console.WriteLine();
    }

}
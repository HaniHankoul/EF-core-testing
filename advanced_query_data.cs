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

    /// <summary>
    /// Loads students with their enrollments and related courses.
    /// </summary>
    public static void ShowStudentsWithEnrollmentsAndCourses(TrainingCenterDbContext context)
    {
        // Build query first
        var query = context.Students
            .Include(s => s.Enrollments)
                .ThenInclude(e => e.Course)
            .OrderBy(s => s.StudentId);

        // Preview SQL before execution
        PreviewSQLUsingToQueryString(query.ToQueryString());

        // Execute query
        var students = query.ToList();

        Console.WriteLine("\nStudents With Enrollments and Courses:");
        Console.WriteLine("--------------------------------------");

        foreach (var student in students)
        {
            Console.WriteLine($"{student.StudentId} - {student.FirstName} {student.LastName}");

            foreach (var enrollment in student.Enrollments)
            {
                Console.WriteLine(
                    $"   Course: {enrollment.Course.Title}, " +
                    $"Status: {enrollment.Status}, " +
                    $"Progress: {enrollment.ProgressPercent}%");
            }

            Console.WriteLine();
        }
    }

    /// <summary>
    /// Shows a course report by joining Courses with Instructors.
    /// </summary>
    public static void ShowCourseReportWithJoin(TrainingCenterDbContext context)
    {
        Console.WriteLine("Course Report Using Join()");
        Console.WriteLine("--------------------------");
        Console.WriteLine();

        // Build query first
        var query =
            context.Courses
                   .Join(
                       context.Instructors,
                       course => course.InstructorId,
                       instructor => instructor.InstructorId,
                       (course, instructor) => new
                       {
                           course.Title,
                           course.Code,
                           InstructorName =
                               instructor.FirstName + " " + instructor.LastName
                       })
                   .OrderBy(x => x.Title);

        // Preview SQL before execution
        PreviewSQLUsingToQueryString(query.ToQueryString());

        // Execute query
        var report = query.ToList();

        // Print readable output
        Console.WriteLine("Courses With Instructors:");
        Console.WriteLine("-------------------------");

        Console.WriteLine();
        foreach (var row in report)
        {
            Console.WriteLine(
                $"{row.Code} - {row.Title} - {row.InstructorName}");
        }

        Console.WriteLine();
        Console.WriteLine($"Total Courses: {report.Count}");
    }

    /// <summary>
    /// Shows students with or without profiles using Left Join.
    /// </summary>
    public static void ShowStudentsWithProfilesWithLeftJoin(TrainingCenterDbContext context)
    {
        Console.WriteLine("Students With Profiles - Left Join");
        Console.WriteLine("----------------------------------");
        Console.WriteLine();

        // Build query first
        var report =
            from s in context.Students
            join p in context.StudentProfiles
                on s.StudentId equals p.StudentId
                into profileGroup
            from p in profileGroup.DefaultIfEmpty()
            select new
            {
                s.StudentId,
                StudentName = s.FirstName + " " + s.LastName,
                City = p != null ? p.City : "No Profile",
                Country = p != null ? p.Country : "No Profile"
            };

        // Apply sorting
        var query =
            report.OrderBy(x => x.StudentId);

        // Preview SQL before execution
        PreviewSQLUsingToQueryString(query.ToQueryString());

        // Execute query
        var result = query.ToList();

        // Print readable output
        Console.WriteLine("Student Report:");
        Console.WriteLine("---------------");

        Console.WriteLine();
        foreach (var row in result)
        {
            Console.WriteLine(
                $"{row.StudentId} - {row.StudentName} - {row.City} - {row.Country}");
        }

        Console.WriteLine();
        Console.WriteLine($"Total Students: {result.Count}");
    }

    /// <summary>
    /// Shows student enrollments by flattening Students -> Enrollments using SelectMany().
    /// </summary>
    public static void ShowStudentEnrollmentsWithSelectMany(TrainingCenterDbContext context)
    {
        Console.WriteLine("Student Enrollments Using SelectMany()");
        Console.WriteLine("--------------------------------------");
        Console.WriteLine();

        // Build query first
        var query =
            context.Students
                   .SelectMany(
                       student => student.Enrollments,
                       (student, enrollment) => new
                       {
                           student.StudentId,
                           StudentName =
                               student.FirstName + " " + student.LastName,
                           enrollment.CourseId,
                           enrollment.Status
                       })
                   .OrderBy(x => x.StudentId);

        // Preview SQL before execution
        PreviewSQLUsingToQueryString(query.ToQueryString());

        // Execute query
        var report = query.ToList();

        Console.WriteLine("Student Course Registrations:");
        Console.WriteLine("-----------------------------");
        Console.WriteLine();

        foreach (var row in report)
        {
            Console.WriteLine(
                $"{row.StudentId} - {row.StudentName} - Course: {row.CourseId} - {row.Status}");
        }

        Console.WriteLine();
        Console.WriteLine($"Total Registrations: {report.Count}");
    }

    /// <summary>
    /// Shows courses priced above the average course price using a subquery.
    /// </summary>
    public static void ShowExpensiveCourses(TrainingCenterDbContext context)
    {
        Console.WriteLine("Courses Priced Above Average");
        Console.WriteLine("----------------------------");
        Console.WriteLine();

        // Build query first
        var query =
            context.Courses
                   .Where(c =>
                       c.Price >
                       context.Courses.Average(x => x.Price))
                   .OrderBy(c => c.Price);

        // Preview SQL before execution
        PreviewSQLUsingToQueryString(query.ToQueryString());

        // Execute query
        // ToQueryString previews query shape,
        // runtime logging shows actual executed SQL for Average().
        var courses = query.ToList();

        // Print readable output
        Console.WriteLine("\nExpensive Courses:");
        Console.WriteLine("------------------");


        Console.WriteLine();
        foreach (var course in courses)
        {
            Console.WriteLine(
                $"{course.Code} - {course.Title} - {course.Price}");
        }

        Console.WriteLine();
        Console.WriteLine($"Total Courses: {courses.Count}");
    }


    private static void PreviewSQLUsingToQueryString(object value)
    {
        Console.WriteLine("\nPreview SQL using ToQueryString():");
        Console.WriteLine("----------------------------------");
        Console.WriteLine(value.ToString());
        Console.WriteLine();
    }
}
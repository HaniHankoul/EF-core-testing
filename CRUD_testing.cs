using System.Diagnostics;
using ConsoleApp1.Models;
using Microsoft.EntityFrameworkCore;

class CRUDtesting
{
    /// <summary>
    /// Demonstrates why AsNoTracking() should not be used when you want EF Core to track updates.
    /// </summary>
    public static void ShowBadUpdateUsingAsNoTracking(TrainingCenterDbContext context)
    {
        Console.WriteLine("BAD APPROACH - Update Using AsNoTracking()");
        Console.WriteLine("------------------------------------------");
        Console.WriteLine();

        // Build query first
        var badQuery =
            context.Students
                   .AsNoTracking()
                   .Where(s => s.StudentId == 1);

        // Preview SQL before execution
        PreviewSQLUsingToQueryString(badQuery.ToQueryString());

        // Execute query
        // ToQueryString previews query shape,
        // runtime logging shows actual executed SQL for FirstOrDefault().
        var badStudent = badQuery.FirstOrDefault();

        if (badStudent == null)
        {
            Console.WriteLine("Student not found.");
            return;
        }

        Console.WriteLine($"Original Name: {badStudent.FirstName}");

        // Modify property in memory only
        badStudent.FirstName = "UpdatedName";

        Console.WriteLine($"Changed Name In Memory: {badStudent.FirstName}");

        // SaveChanges() will not update this entity because it is not tracked
        int affectedRows = context.SaveChanges();

        Console.WriteLine();
        Console.WriteLine($"Affected Rows: {affectedRows}");
        Console.WriteLine("Nothing was updated because the entity was loaded using AsNoTracking().");
        Console.WriteLine();
    }


    /// <summary>
    /// Demonstrates the correct update approach using a tracked entity.
    /// </summary>
    public static void ShowGoodUpdateUsingTrackedEntity(TrainingCenterDbContext context)
    {
        Console.WriteLine("GOOD APPROACH - Update Using Tracked Entity");
        Console.WriteLine("-------------------------------------------");
        Console.WriteLine();

        // Build query first
        var goodQuery =
            context.Students
                   .Where(s => s.StudentId == 1);

        // Preview SQL before execution
        PreviewSQLUsingToQueryString(goodQuery.ToQueryString());

        // Execute query
        // ToQueryString previews query shape,
        // runtime logging shows actual executed SQL for FirstOrDefault().
        var student = goodQuery.FirstOrDefault();

        if (student == null)
        {
            Console.WriteLine("Student not found.");
            return;
        }

        Console.WriteLine($"Before Update: {student.FirstName}");

        // Modify property while entity is tracked by EF Core
        student.FirstName = "Hani";

        Console.WriteLine($"After Change In Memory: {student.FirstName}");

        // Save changes
        // Runtime logging shows the actual executed UPDATE SQL.
        int affectedRows = context.SaveChanges();

        Console.WriteLine();
        Console.WriteLine($"Affected Rows: {affectedRows}");
        Console.WriteLine("Changes saved to database.");
        Console.WriteLine();
    }

    /// <summary>
    /// Inserts a new student and returns the generated StudentId.
    /// </summary>
    public static int InsertStudent(TrainingCenterDbContext context)
    {
        Console.WriteLine("INSERT EXAMPLE");
        Console.WriteLine("--------------");
        Console.WriteLine();

        // Create a new Student entity in memory
        var newStudent = new Student
        {
            FirstName = "Ali",
            LastName = "Hassan",
            Email = "ali.hassan@example.com",
            Status = "Active",
            RegisteredAt = DateTime.Now
        };

        // Add() tells EF Core to track this entity as Added
        context.Students.Add(newStudent);

        Console.WriteLine("Student added to Change Tracker as Added.");
        Console.WriteLine("INSERT does not support ToQueryString().");
        Console.WriteLine("Runtime logging will show the actual executed INSERT SQL.");
        Console.WriteLine();

        // Execute INSERT
        int affectedRows = context.SaveChanges();

        Console.WriteLine($"Affected Rows       : {affectedRows}");
        Console.WriteLine($"Generated Student ID: {newStudent.StudentId}");
        Console.WriteLine("Student inserted successfully.");
        Console.WriteLine();

        return newStudent.StudentId;
    }

    /// <summary>
    /// Deletes a student by StudentId.
    /// </summary>
    public static void DeleteStudent(TrainingCenterDbContext context, int studentId)
    {
        Console.WriteLine("DELETE EXAMPLE");
        Console.WriteLine("--------------");
        Console.WriteLine();

        // Build query first
        var query =
            context.Students
                   .Where(s => s.StudentId == studentId);

        // Preview SELECT SQL before execution
        PreviewSQLUsingToQueryString(query.ToQueryString());

        // Execute query
        // ToQueryString previews query shape,
        // runtime logging shows actual executed SQL for FirstOrDefault().
        var student = query.FirstOrDefault();

        if (student == null)
        {
            Console.WriteLine($"No student found with ID = {studentId}");
            return;
        }

        Console.WriteLine(
            $"Deleting Student: {student.FirstName} {student.LastName} (ID = {student.StudentId})");

        // Remove() tells EF Core to track this entity as Deleted
        context.Students.Remove(student);

        Console.WriteLine();
        Console.WriteLine("Student marked as Deleted in Change Tracker.");
        Console.WriteLine("DELETE does not support ToQueryString().");
        Console.WriteLine("Runtime logging will show the actual executed DELETE SQL.");
        Console.WriteLine();

        // Execute DELETE
        int affectedRows = context.SaveChanges();

        Console.WriteLine($"Affected Rows: {affectedRows}");
        Console.WriteLine("Student deleted successfully.");
        Console.WriteLine();
    }


    /// <summary>
    /// Demonstrates the bad bulk insert approach by calling SaveChanges() inside the loop.
    /// </summary>
    static long RunBadBulkInsert(TrainingCenterDbContext context)
    {
        Console.WriteLine("BAD BULK INSERT - SaveChanges() Inside Loop");
        Console.WriteLine("-------------------------------------------");
        Console.WriteLine();

        var stopwatch = Stopwatch.StartNew();

        for (int i = 1; i <= 10; i++)
        {
            var student = new Student
            {
                FirstName = "Bad",
                LastName = $"Student{i}",
                Email = $"bad{i}@test.com",
                Status = "Active",
                RegisteredAt = DateTime.Now
            };

            context.Students.Add(student);

            // BAD: SaveChanges() inside the loop creates many database calls
            int affectedRows = context.SaveChanges();

            Console.WriteLine(
                $"Inserted Student {i} | Generated ID: {student.StudentId} | Affected Rows: {affectedRows}");
        }

        stopwatch.Stop();

        Console.WriteLine();
        Console.WriteLine($"Time Taken - BAD: {stopwatch.ElapsedMilliseconds} ms");
        Console.WriteLine("Result: Many database calls were executed.");
        Console.WriteLine();

        return stopwatch.ElapsedMilliseconds;
    }


    /// <summary>
    /// Demonstrates the good bulk insert approach by adding all entities first,
    /// then calling SaveChanges() only once.
    /// </summary>
    static long RunGoodBulkInsert(TrainingCenterDbContext context)
    {
        Console.WriteLine("GOOD BULK INSERT - Single SaveChanges()");
        Console.WriteLine("---------------------------------------");
        Console.WriteLine();

        var stopwatch = Stopwatch.StartNew();

        var students = new List<Student>();

        for (int i = 1; i <= 10; i++)
        {
            students.Add(new Student
            {
                FirstName = "Good",
                LastName = $"Student{i}",
                Email = $"good{i}@test.com",
                Status = "Active",
                RegisteredAt = DateTime.Now
            });
        }

        context.Students.AddRange(students);

        Console.WriteLine("Students added to Change Tracker as Added.");
        Console.WriteLine("INSERT does not support ToQueryString().");
        Console.WriteLine("Runtime logging will show the actual executed INSERT SQL.");
        Console.WriteLine();

        int affectedRows = context.SaveChanges();

        stopwatch.Stop();

        Console.WriteLine($"Affected Rows: {affectedRows}");
        Console.WriteLine($"Time Taken - GOOD: {stopwatch.ElapsedMilliseconds} ms");
        Console.WriteLine("Result: One SaveChanges() call was executed.");
        Console.WriteLine();

        return stopwatch.ElapsedMilliseconds;
    }


    /// <summary>
    /// Demonstrates the best bulk insert approach using EFCore.BulkExtensions.
    /// </summary>
    static long RunBestBulkInsertUsingBulkExtensions(TrainingCenterDbContext context)
    {
        Console.WriteLine("BEST BULK INSERT - EFCore.BulkExtensions");
        Console.WriteLine("----------------------------------------");
        Console.WriteLine();

        var stopwatch = Stopwatch.StartNew();

        var students = new List<Student>();

        for (int i = 1; i <= 10; i++)
        {
            students.Add(new Student
            {
                FirstName = "Bulk",
                LastName = $"Student{i}",
                Email = $"bulk{i}@test.com",
                Status = "Active",
                RegisteredAt = DateTime.Now
            });
        }

        Console.WriteLine("Students prepared in memory.");
        Console.WriteLine("BulkInsert() performs optimized database bulk operation.");
        Console.WriteLine("BulkInsert() does not use SaveChanges().");
        Console.WriteLine();

        //context.BulkInsert(students);

        stopwatch.Stop();

        Console.WriteLine($"Inserted Rows: {students.Count}");
        Console.WriteLine($"Time Taken - BULK: {stopwatch.ElapsedMilliseconds} ms");
        Console.WriteLine("Result: Optimized bulk insert operation was executed.");
        Console.WriteLine();

        return stopwatch.ElapsedMilliseconds;
    }


    /// <summary>
    /// Shows a simple performance comparison between the bad, good, and bulk approaches.
    /// </summary>
    static void ShowComparison(long badTime, long goodTime, long bulkTime)
    {
        Console.WriteLine("COMPARISON");
        Console.WriteLine("----------");
        Console.WriteLine($"BAD  Time : {badTime} ms");
        Console.WriteLine($"GOOD Time : {goodTime} ms");
        Console.WriteLine($"BULK Time : {bulkTime} ms");
        Console.WriteLine();
        Console.WriteLine("Fewer SaveChanges() calls usually means better performance.");
        Console.WriteLine("BulkExtensions is usually best for large datasets.");
    }

    public static void TransactionDemo(TrainingCenterDbContext context)
    {
        using var transaction =
    context.Database.BeginTransaction();

        try
        {
            // Find sender
            var mohammed = context.Students
                .First(a => a.FirstName == "Mohammed");

            // Find receiver
            var ali = context.Students
                .First(a => a.LastName == "Ali");

            // Withdraw
            //mohammed.Balance -= 100;

            // Deposit
            //ali.Balance += 100;

            // Save both changes
            context.SaveChanges();

            // Confirm transaction
            transaction.Commit();

            Console.WriteLine("Transfer completed successfully.");
        }
        catch
        {
            // Cancel all changes
            transaction.Rollback();

            Console.WriteLine("Transfer failed. No money moved.");
        }
    }
    static void PrintSeparator()
    {
        Console.WriteLine(new string('-', 60));
        Console.WriteLine();
    }
    static void PreviewSQLUsingToQueryString(string SQLString)
    {
        Console.WriteLine("\nPreview SQL using ToQueryString():");
        Console.WriteLine("----------------------------------");
        Console.WriteLine(SQLString);
        Console.WriteLine();
    }
}
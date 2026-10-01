/*
========================================================
Code Overview
--------------------------------------------------------
Purpose:
- Read connection string from appsettings.json
- Create AppDbContext manually
- Retrieve students
- Preview SQL using ToQueryString()
- Log actual executed SQL
- Compare ToQueryString() vs runtime SQL for Count()

Key Points:
- ToQueryString previews IQueryable shape
- Logging shows runtime SQL
- Count() final SQL differs from preview SQL
========================================================
*/

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ConsoleApp1.Models;

// Build configuration
IConfiguration configuration =
    new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", false, true)
        .Build();

// Read connection string
string? connectionString =
    configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrWhiteSpace(connectionString))
{
    Console.WriteLine("Connection string not found.");
    return;
}

// Create options with logging
var options =
    new DbContextOptionsBuilder<TrainingCenterDbContext>()
        .UseSqlServer(connectionString)
        .LogTo(Console.WriteLine, LogLevel.Information)
        .EnableSensitiveDataLogging()
        .Options;

// Create context
using var context = new TrainingCenterDbContext(options);

// Test connection
if (!context.Database.CanConnect())
{
    Console.WriteLine("Could not connect.");
    return;
}

Console.WriteLine("Connected successfully.");
Console.WriteLine();

// Run examples
AdvancedQueryData.ShowCourseReportWithJoin(context);


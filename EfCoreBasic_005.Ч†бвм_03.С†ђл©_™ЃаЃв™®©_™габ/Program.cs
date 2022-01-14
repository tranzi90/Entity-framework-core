using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EfCoreBasic_005.Часть_03.Самый_короткий_курс
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Создание_пустой_базы_данных();
            Добавление_данных();
            Чтение_данных();
        }

        public static void Создание_пустой_базы_данных()
        {
            using var dbContext = new ApplicationDbContext();

            dbContext.Database.EnsureDeleted();

            dbContext.Database.EnsureCreated();
        }

        public static void Добавление_данных()
        {
            using var dbContext = new ApplicationDbContext();

            var csharpCourse = new Course
            {
                Name = "C# Advanced",
                LessonsQuantity = 7,
                Duration = TimeSpan.FromHours(7),
            };

            var efCoreCourse = new Course
            {
                Name = "Entity Framework Core Basic",
                LessonsQuantity = 10,
                Duration = TimeSpan.FromHours(10),
            };

            var unitTestsCourse = new Course
            {
                Name = "Юнит-тестирование",
                LessonsQuantity = 3,
                Duration = TimeSpan.FromHours(3)
            };

            dbContext.Add(csharpCourse);
            dbContext.Add(efCoreCourse);
            dbContext.Add(unitTestsCourse);

            dbContext.SaveChanges();
        }

        public static void Чтение_данных()
        {
            using var dbContext = new ApplicationDbContext();

            // так как мы сразу материализуем результат, сгенерированный SQL будет виден в логах
            var course = dbContext
                .Courses
                .OrderBy(x => x.Duration)
                .FirstOrDefault();

            Console.WriteLine(
                new string(
                    '-',
                    80));
            Console.WriteLine(
                $"Название курса: {course.Name}. " +
                $"Количество уроков: {course.LessonsQuantity}. " +
                $"Длительность: {course.Duration}.");
        }
    }

    public class ApplicationDbContext : DbContext
    {
        public DbSet<Course> Courses { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseSqlServer(
                    @"Server=(localdb)\mssqllocaldb;Database=EfCoreBasicDb;Trusted_Connection=True;")
                .EnableDetailedErrors()
                .EnableSensitiveDataLogging()
                // логирование добавлено, что-бы увидеть сгенерированный SQL
                .LogTo(
                    Console.WriteLine,
                    new[] { DbLoggerCategory.Database.Command.Name },
                    LogLevel.Information);
        }
    }

    public class Course
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int LessonsQuantity { get; set; }

        public TimeSpan Duration { get; set; }
    }
}
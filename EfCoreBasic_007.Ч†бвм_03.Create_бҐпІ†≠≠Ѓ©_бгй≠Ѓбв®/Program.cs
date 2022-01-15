using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EfCoreBasic_007.Часть_03.Create_связанной_сущности
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Создание_пустой_базы_данных();
            Добавление_данных();
            Чтение_данных();
            Добавление_связанной_сущности();
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
                LessonsQuantity = 7
            };

            var efCoreCourse = new Course
            {
                Name = "Entity Framework Core Basic",
                LessonsQuantity = 10,
            };

            var johnSmith = new Author
            {
                FirstName = "John",
                LastName = "Smith"
            };

            var arthurMorgan = new Author
            {
                FirstName = "Arthur",
                LastName = "Morgan"
            };

            // EF Core автоматически отследит связь между сущностями
            csharpCourse.Author = johnSmith;
            efCoreCourse.Author = johnSmith;

            // достаточно добавить только уникальные сущности
            dbContext.Add(csharpCourse);
            dbContext.Add(efCoreCourse);
            dbContext.Add(arthurMorgan);

            // в итоге EF Core добавит граф сущностей
            // два автора и два курса, связанные между собой
            dbContext.SaveChanges();
        }

        public static void Чтение_данных()
        {
            using var dbContext = new ApplicationDbContext();

            var courses = dbContext
                .Courses
                .Include(x => x.Author)
                .ToList();

            Console.WriteLine(
                new string(
                    '-',
                    80));
            Console.WriteLine("Информация о курсах.");
            foreach (var course in courses)
            {
                Console.WriteLine(
                    $"Имя курса: {course.Name}. " +
                    $"Идентификатор курса: {course.Id}. " +
                    $"Количество уроков: {course.LessonsQuantity}. " +
                    $"Автор: {course.Author.FirstName + " " + course.Author.LastName}.");
            }

            Console.WriteLine(
                new string(
                    '-',
                    80));

            var authors = dbContext
                .Authors
                .ToList();

            Console.WriteLine(
                new string(
                    '-',
                    80));
            Console.WriteLine("Информация об авторах.");
            foreach (var author in authors)
            {
                Console.WriteLine($"Автор: {author.FirstName + " " + author.LastName}");
            }

            Console.WriteLine(
                new string(
                    '-',
                    80));
        }

        public static void Добавление_связанной_сущности()
        {
            using var dbContext = new ApplicationDbContext();

            var unitTestsCourse = new Course
            {
                Name = "Юнит-тестирование",
                LessonsQuantity = 3,
            };

            var author = dbContext
                .Authors
                // необходимо загрузить курсы
                // для того, чтобы EF Core правильно обработал добавление нового курса
                .Include(x => x.Courses)
                .Single(x => x.FirstName == "Arthur");

            // добавляем новый курс
            // EF Core автоматически распознает его как новый
            author.Courses.Add(unitTestsCourse);

            dbContext.SaveChanges();
        }
    }

    public class ApplicationDbContext : DbContext
    {
        public DbSet<Course> Courses { get; set; }

        public DbSet<Author> Authors { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseSqlServer(
                    @"Server=(localdb)\mssqllocaldb;Database=EfCoreBasicDb;Trusted_Connection=True;")
                .EnableDetailedErrors()
                .EnableSensitiveDataLogging()
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

        public Author Author { get; set; }
    }

    public class Author
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public ICollection<Course> Courses { get; set; }
    }
}
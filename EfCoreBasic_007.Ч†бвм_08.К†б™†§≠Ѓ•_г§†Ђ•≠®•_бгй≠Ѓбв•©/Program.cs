using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EfCoreBasic_007.Часть_08.Каскадное_удаление_сущностей
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Создание_пустой_базы_данных();
            Добавление_данных();
            Чтение_данных();
            Каскадное_удаление();
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

            var unitTestsCourse = new Course
            {
                Name = "Юнит-тестирование",
                LessonsQuantity = 3,
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

            csharpCourse.Author = johnSmith;
            efCoreCourse.Author = johnSmith;
            unitTestsCourse.Author = arthurMorgan;

            dbContext.Add(csharpCourse);
            dbContext.Add(efCoreCourse);
            dbContext.Add(unitTestsCourse);
            dbContext.Add(arthurMorgan);

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

        public static void Каскадное_удаление()
        {
            using var dbContext = new ApplicationDbContext();

            var johnSmith = dbContext
                .Authors
                .Single(x => x.FirstName == "John");

            // Просто удаляем автора
            dbContext.Remove(johnSmith);

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

        // это свойство NOT NULL
        // поэтому по умолчанию EF Core настраивает каскадное удаление
        // в итоге если удалим автора, автоматически удаляются все его курсы
        public int AuthorId { get; set; }
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
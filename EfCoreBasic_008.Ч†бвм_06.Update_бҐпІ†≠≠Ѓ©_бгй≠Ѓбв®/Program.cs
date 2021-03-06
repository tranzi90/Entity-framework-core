using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace EfCoreBasic_008.Часть_06.Update_связанной_сущности
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Создание_пустой_базы_данных();
            Добавление_данных();
            Обновление_связанной_сущности();
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
            unitTestsCourse.Author = johnSmith;

            dbContext.Add(csharpCourse);
            dbContext.Add(efCoreCourse);
            dbContext.Add(unitTestsCourse);
            dbContext.Add(arthurMorgan);

            dbContext.SaveChanges();
        }

        public static void Обновление_связанной_сущности()
        {
            using var dbContext = new ApplicationDbContext();

            var author = dbContext
                .Authors
                .Single(x => x.FirstName == "Arthur");

            var courses = dbContext
                .Courses
                .Include(x => x.Author)
                .ToList();

            Console.WriteLine(
                new string(
                    '-',
                    80));
            Console.WriteLine(dbContext.ChangeTracker.DebugView.LongView);

            foreach (var course in courses)
            {
                // EF Core автоматически отследит что у курса изменился автор
                course.Author = author;
            }

            Console.WriteLine(
                new string(
                    '-',
                    80));
            Console.WriteLine(dbContext.ChangeTracker.DebugView.LongView);

            dbContext.SaveChanges();

            Console.WriteLine(
                new string(
                    '-',
                    80));
            Console.WriteLine(dbContext.ChangeTracker.DebugView.LongView);
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
                    @"Server=(localdb)\mssqllocaldb;Database=EfCoreBasicDb;Trusted_Connection=True;");
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
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace EfCoreBasic_008.Часть_10.Delete_сущности_вне_контекста
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Создание_пустой_базы_данных();
            Добавление_данных();
            Удаление_сущности_вне_контекста();
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

        public static void Удаление_сущности_вне_контекста()
        {
            using var dbContext = new ApplicationDbContext();

            var firstCourse = new Course
            {
                Id = 1
            };

            Console.WriteLine(
                new string(
                    '-',
                    80));
            Console.WriteLine(
                dbContext.ChangeTracker.DebugView.LongView == ""
                    ? "Контекст пустой."
                    : dbContext.ChangeTracker.DebugView.LongView);

            dbContext.Remove(firstCourse);

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
            Console.WriteLine(
                dbContext.ChangeTracker.DebugView.LongView == ""
                    ? "Контекст пустой."
                    : dbContext.ChangeTracker.DebugView.LongView);
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
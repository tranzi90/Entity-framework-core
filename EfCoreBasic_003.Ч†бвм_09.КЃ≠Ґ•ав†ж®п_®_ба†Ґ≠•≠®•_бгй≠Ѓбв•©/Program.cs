using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Logging;

namespace EfCoreBasic_003.Часть_09.Конвертация_и_сравнение_сущностей
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Создание_пустой_базы_данных();
            Добавление_данных();
            Чтение_данных();
            Изменение_данных();
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

            var course = new Course
            {
                Name = "Entity Framework Core Базовый",
                Tags = new[]
                {
                    "EF Core",
                    "Basic"
                },
            };

            dbContext.Add(course);

            dbContext.SaveChanges();
        }

        public static void Чтение_данных()
        {
            using var dbContext = new ApplicationDbContext();

            var course = dbContext.Courses.First();

            Console.WriteLine(
                new string(
                    '-',
                    80));
            Console.WriteLine(
                $"Имя курса: {course.Name}. " +
                $"Теги курса: {string.Join(";", course.Tags)}. ");
            Console.WriteLine(
                new string(
                    '-',
                    80));
        }

        public static void Изменение_данных()
        {
            using var dbContext = new ApplicationDbContext();

            var course = dbContext.Courses.First();

            course.Tags = new[]
            {
                "EF Core",
                "CSharp"
            };

            dbContext.SaveChanges();
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
                .LogTo(
                    Console.WriteLine,
                    new[] { DbLoggerCategory.Database.Command.Name },
                    LogLevel.Information);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var courseTagsConverter = new ValueConverter<string[], string>(
                // конвертируем из тегов в строку
                v => string.Join(
                    ";",
                    v),
                // конвертируем из строки в теги
                v => v.Split(
                    ';',
                    StringSplitOptions.RemoveEmptyEntries).ToArray());

            var courseTagsValueComparer = new ValueComparer<string[]>(
                // переопределяем Equals
                (x, y) => x.SequenceEqual(
                    y,
                    StringComparer.OrdinalIgnoreCase),
                // переопределяем GetHashCode
                x => x.Aggregate(
                    0,
                    (a, v) => HashCode.Combine(
                        a,
                        v.GetHashCode(StringComparison.OrdinalIgnoreCase))),
                // специальное выражение для создания снепшота данных
                // (подробнее об этом будет в уроке про ChangeTracking)
                x => x.ToArray());

            modelBuilder
                .Entity<Course>()
                .Property(t => t.Tags)
                .HasConversion(
                    courseTagsConverter,
                    courseTagsValueComparer);
        }
    }

    public class Course
    {
        public int Id { get; set; }

        public string Name { get; set; }

        // теги курса, которые мы хотим сохранить в одну колонку
        public string[] Tags { get; set; }
    }
}
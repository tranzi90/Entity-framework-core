using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace EfCoreBasic_005.Часть_02.Фильтрация_и_сортировка
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

            var coursesQueryable = dbContext
                .Courses
                .Where(x => x.LessonsQuantity > 5)
                .OrderByDescending(x => x.Duration);

            var courses = coursesQueryable.ToList();

            Console.WriteLine(
                new string(
                    '-',
                    80));

            foreach (var course in courses)
            {
                Console.WriteLine(
                    $"Название курса: {course.Name}. " +
                    $"Количество уроков: {course.LessonsQuantity}. " +
                    $"Длительность: {course.Duration}.");
            }

            var generatedSql = coursesQueryable.ToQueryString();

            Console.WriteLine(
                new string(
                    '-',
                    80));

            Console.WriteLine("Сгенерированный SQL запрос:");
            Console.WriteLine(generatedSql);
        }
    }

    public class ApplicationDbContext : DbContext
    {
        public DbSet<Course> Courses { get; set; }

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

        public TimeSpan Duration { get; set; }
    }
}
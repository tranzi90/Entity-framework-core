using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EfCoreBasic_005.Часть_05.Клиентское_выполнение_запроса
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
                LessonsQuantity = 7
            };

            var efCoreCourse = new Course
            {
                Name = "Entity Framework Core Basic",
                LessonsQuantity = 10,
            };

            dbContext.Add(csharpCourse);
            dbContext.Add(efCoreCourse);

            dbContext.SaveChanges();
        }

        public static void Чтение_данных()
        {
            using var dbContext = new ApplicationDbContext();

            var coursesQueryable = dbContext
                .Courses
                // Select будет выполняться уже на клиентской стороне, а не на стороне БД
                .Select(
                    x => new
                    {
                        x.Name,
                        x.LessonsQuantity,
                        Announcement = CreateAnnouncement(
                            x.Name,
                            x.LessonsQuantity),
                    })
                .OrderBy(x => x.LessonsQuantity);

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
                    $"Объявление о новом курсе: {course.Announcement}.");
            }

            var generatedSql = coursesQueryable.ToQueryString();

            Console.WriteLine(
                new string(
                    '-',
                    80));

            Console.WriteLine("Сгенерированный SQL запрос:");
            Console.WriteLine(generatedSql);
        }

        public static string CreateAnnouncement(string name, int quantity)
            => $"Новый курс {name} с количеством уроков {quantity}!";
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
    }

    public class Course
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int LessonsQuantity { get; set; }
    }
}
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EfCoreBasic_003.Часть_12.Сущности_без_ключа
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Создание_пустой_базы_данных();
            Создание_View();
            Добавление_данных();
            Чтение_данных();
        }

        public static void Создание_пустой_базы_данных()
        {
            using var dbContext = new ApplicationDbContext();

            dbContext.Database.EnsureDeleted();

            dbContext.Database.EnsureCreated();
        }

        public static void Создание_View()
        {
            using var dbContext = new ApplicationDbContext();

            var createViewCommand = @$"
                CREATE VIEW LongCourses AS
                    SELECT c.Name as CourseName
                    FROM Courses c
                    WHERE c.LessonsQuantity > 5";

            // создаём View в БД, выполняя команду выше
            dbContext.Database.ExecuteSqlRaw(createViewCommand);
        }

        public static void Добавление_данных()
        {
            using var dbContext = new ApplicationDbContext();

            var csharpCourse = new Course
            {
                Name = "C# Basic",
                LessonsQuantity = 3
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

            var course = dbContext.LongCourses.First();

            Console.WriteLine(
                new string(
                    '-',
                    80));
            Console.WriteLine("Информация о курсе.");
            Console.WriteLine($"Имя курса: {course.CourseName}.");
            Console.WriteLine(
                new string(
                    '-',
                    80));
        }
    }

    public class ApplicationDbContext : DbContext
    {
        public DbSet<Course> Courses { get; set; }

        public DbSet<LongCourse> LongCourses { get; set; }

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
            modelBuilder
                .Entity<LongCourse>()
                // обязательно нужно указать что у этой сущности нет ключа
                // иначе будут ошибки от EF Core
                .HasNoKey();

            modelBuilder
                .Entity<LongCourse>()
                // настраиваем отображение в View
                .ToView("LongCourses");
        }
    }

    public class Course
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int LessonsQuantity { get; set; }
    }

    // сущность без ключа
    public class LongCourse
    {
        public string CourseName { get; set; }
    }
}
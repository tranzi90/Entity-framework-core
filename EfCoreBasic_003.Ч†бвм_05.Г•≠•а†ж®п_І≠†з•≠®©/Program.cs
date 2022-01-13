using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EfCoreBasic_003.Часть_05.Генерация_значений
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
                LessonsQuantity = 10
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
                $"Количество уроков: {course.LessonsQuantity}. " +
                $"Заголовок: {course.Title}.");
            Console.WriteLine(
                new string(
                    '-',
                    80));
        }

        public static void Изменение_данных()
        {
            using var dbContext = new ApplicationDbContext();

            var course = dbContext.Courses.First();

            course.LessonsQuantity = 7;

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

        // здесь мы настраиваем моделирование сущностей в EF Core
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Course>()
                .Property(t => t.Title)
                // передаём SQL выражение для вычисления значения
                // также передаём флаг, храним колонку или нет
                .HasComputedColumnSql(
                    "[Name] + ' contains ' + CAST([LessonsQuantity] AS NVARCHAR) + ' lessons.'",
                    stored: true);
        }
    }

    public class Course
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int LessonsQuantity { get; set; }

        public string Title { get; set; }
    }
}
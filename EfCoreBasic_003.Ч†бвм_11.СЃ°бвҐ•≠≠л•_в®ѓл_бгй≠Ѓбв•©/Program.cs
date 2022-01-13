using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EfCoreBasic_003.Часть_11.Собственные_типы_сущностей
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

            var course = new Course
            {
                Name = "Entity Framework Core Базовый",
                LessonsQuantity = 10,
                CreatedAt = new DateTimeOffset(
                    2007,
                    1,
                    1,
                    1,
                    1,
                    1,
                    TimeSpan.Zero),
                FinancialCourseInfo = new FinancialCourseInfo
                {
                    Price = 15
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
            Console.WriteLine("Информация о курсе.");
            Console.WriteLine(
                $"Имя курса: {course.Name}. " +
                $"Количество уроков: {course.LessonsQuantity}. " +
                $"Цена: {course.FinancialCourseInfo.Price}.");
            Console.WriteLine(
                new string(
                    '-',
                    80));
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
            modelBuilder
                .Entity<Course>()
                // указываем что у курса есть собственная сущность - финансовая информация курса
                .OwnsOne(
                    t => t.FinancialCourseInfo,
                    t =>
                    {
                        // указываем что у финансовой информации курса есть владелец, а это значит что эта сущность является собственной
                        t.WithOwner();
                    });
        }
    }

    public class Course
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int LessonsQuantity { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public FinancialCourseInfo FinancialCourseInfo { get; set; }
    }

    // модель финансовой информации курса
    // находится в той же таблице что и модель курса
    // но теперь это собственный тип сущности
    public class FinancialCourseInfo
    {
        public decimal Price { get; set; }
    }
}
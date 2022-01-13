using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EfCoreBasic_003.Часть_13.One_to_one
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
            };

            var courseFinancialInfo = new FinancialCourseInfo
            {
                Id = course.Id,
                Price = 15M
            };

            course.FinancialCourseInfo = courseFinancialInfo;

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
                $"Идентификатор курса: {course.Id}. " +
                $"Количество уроков: {course.LessonsQuantity}. " +
                $"Цена: {course.FinancialCourseInfo?.Price ?? -1}.");
            Console.WriteLine(
                new string(
                    '-',
                    80));

            var financialCourseInfo = dbContext.FinancialCourseInfos.First();

            Console.WriteLine(
                new string(
                    '-',
                    80));
            Console.WriteLine("Финансовая информация о курсе.");
            Console.WriteLine(
                $"Идентификатор курса: {financialCourseInfo.CourseId}. " +
                $"Цена: {financialCourseInfo.Price}.");
            Console.WriteLine(
                new string(
                    '-',
                    80));
        }
    }

    public class ApplicationDbContext : DbContext
    {
        public DbSet<Course> Courses { get; set; }

        public DbSet<FinancialCourseInfo> FinancialCourseInfos { get; set; }

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
                // указываем что у курса есть одно навигационное свойство с финансовой информацией
                .HasOne(t => t.FinancialCourseInfo)
                // указываем что у финансовой информации есть навигационное свойство с курсом
                .WithOne(t => t.Course)
                // указываем что финансовая информация связана с курсом через такой внешний ключ
                .HasForeignKey<FinancialCourseInfo>(t => t.CourseId);
        }
    }

    public class Course
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int LessonsQuantity { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public int FinancialCourseInfoId { get; set; }
        public FinancialCourseInfo FinancialCourseInfo { get; set; }
    }

    // модель финансовой информации курса
    // находится в в другой таблице
    // у каждого курса есть только одна финансовая информация и наоборот
    public class FinancialCourseInfo
    {
        public int Id { get; set; }

        public decimal Price { get; set; }

        public int CourseId { get; set; }
        public Course Course { get; set; }
    }
}
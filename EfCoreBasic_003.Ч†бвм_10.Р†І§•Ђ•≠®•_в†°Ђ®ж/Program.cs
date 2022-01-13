using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EfCoreBasic_003.Часть_10.Разделение_таблиц
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
                Name = course.Name,
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
                $"Имя курса: {financialCourseInfo.Name}. " +
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // явно указываем что обе эти сущности принадлежат к одной таблице
            modelBuilder
                .Entity<Course>()
                .ToTable("Courses");

            modelBuilder
                .Entity<FinancialCourseInfo>()
                .ToTable("Courses");

            // для настройки разделения таблиц обязательно необходимо написать эту конфигурацию
            modelBuilder
                .Entity<Course>()
                // указываем что у курса есть финансовая информация 
                .HasOne(x => x.FinancialCourseInfo)
                // связь между ними один к одному
                .WithOne()
                // обязательно нужно указать этот внешний ключ, чтобы EF Core понял что здесь настраивается именно разделение таблиц
                .HasForeignKey<FinancialCourseInfo>(x => x.Id);

            // настраиваем общее свойство
            modelBuilder
                .Entity<Course>()
                .Property(t => t.Name)
                .HasColumnName("Name");
            modelBuilder
                .Entity<FinancialCourseInfo>()
                .Property(t => t.Name)
                .HasColumnName("Name");
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
    public class FinancialCourseInfo
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }
    }
}
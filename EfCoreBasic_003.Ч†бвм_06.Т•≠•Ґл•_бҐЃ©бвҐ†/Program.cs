using System;
using System.Linq;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EfCoreBasic_003.Часть_06.Теневые_свойства
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Создание_пустой_базы_данных();
            Добавление_данных();
            Чтение_данных();

            Thread.Sleep(TimeSpan.FromSeconds(30));

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

            var course = dbContext.Courses
                .Select(
                    x => new
                    {
                        x.Name,
                        x.LessonsQuantity,
                        // специфический синтаксис доступа к теневым свойствам
                        // работает только в LINQ запросах
                        CreatedAt = EF.Property<DateTimeOffset>(
                            x,
                            "CreatedAt"),
                        UpdatedAt = EF.Property<DateTimeOffset>(
                            x,
                            "UpdatedAt"),
                    })
                .First();

            Console.WriteLine(
                new string(
                    '-',
                    80));
            Console.WriteLine(
                $"Имя курса: {course.Name}. " +
                $"Количество уроков: {course.LessonsQuantity}. " +
                $"Дата создания: {course.CreatedAt}. " +
                $"Дата изменения: {course.UpdatedAt}.");
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Course>()
                .Property<DateTimeOffset>("CreatedAt")
                // значением по умолчанию будет текущее время
                .HasDefaultValueSql("GETUTCDATE()")
                // подсказываем EF Core, что это значения обновляется при сохранении новой сущности
                .ValueGeneratedOnAdd();

            modelBuilder
                .Entity<Course>()
                .Property<DateTimeOffset>("UpdatedAt")
                // значением по умолчанию будет текущее время
                .HasDefaultValueSql("GETUTCDATE()")
                // подсказываем EF Core, что это значения обновляется при сохранении новой сущности или обновлении старой
                // заметьте, что EF Core автоматически не будет обновлять эту колонку!
                // необходимо явно прописывать логику по обновлению этого значения при обновлении старой сущности
                .ValueGeneratedOnAddOrUpdate();
        }
    }

    public class Course
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int LessonsQuantity { get; set; }
    }
}
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EfCoreBasic_003.Часть_08.Резервные_поля
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
                Duration = TimeSpan.FromHours(5),
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
                $"Длительность: {course.Duration}. ");
            Console.WriteLine(
                new string(
                    '-',
                    80));
        }

        public static void Изменение_данных()
        {
            using var dbContext = new ApplicationDbContext();

            var course = dbContext.Courses.First();

            course.Duration = TimeSpan.FromDays(1);

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
                .Property(t => t.Duration)
                // указываем имя резервного поля
                .HasField("_validatedDuration");
        }
    }

    public class Course
    {
        public int Id { get; set; }

        public string Name { get; set; }

        // резервное поле
        private TimeSpan _validatedDuration;

        // свойство, через которое мы получаем доступ к резервному полю
        public TimeSpan Duration
        {
            get => _validatedDuration;
            set => _validatedDuration =
                value > TimeSpan.FromHours(10) ? TimeSpan.FromHours(10) : value;
        }
    }
}
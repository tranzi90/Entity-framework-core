using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace EfCoreBasic_008.Часть_01.Простое_чтение
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

            Console.WriteLine(
                new string(
                    '-',
                    80));

            // смотрим что находится в трекере изменений
            // если в трекере изменений ничего, возвращается пустая строка
            Console.WriteLine(
                dbContext.ChangeTracker.DebugView.LongView == ""
                    ? "Контекст пустой."
                    : dbContext.ChangeTracker.DebugView.LongView);

            Console.WriteLine(
                new string(
                    '-',
                    80));

            var courses = dbContext.Courses.ToList();

            Console.WriteLine(
                new string(
                    '-',
                    80));

            Console.WriteLine(dbContext.ChangeTracker.DebugView.LongView);
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
    }
}
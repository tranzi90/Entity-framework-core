using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace EfCoreBasic_005.Часть_01.Простое_чтение
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

            // добавляем курсы в контекст EF Core
            // здесь EF Core ещё не выполняет никаких запросов в БД
            dbContext.Add(csharpCourse);
            dbContext.Add(efCoreCourse);

            // уже здесь EF Core выполняет команду, которая сохраняет курсы в БД
            dbContext.SaveChanges();
        }

        public static void Чтение_данных()
        {
            using var dbContext = new ApplicationDbContext();

            // читаем ВСЕ курсы из БД
            var courses = dbContext.Courses.ToList();

            Console.WriteLine(
                new string(
                    '-',
                    80));

            foreach (var course in courses)
            {
                Console.WriteLine(
                    $"Название курса: {course.Name}. " +
                    $"Количество уроков: {course.LessonsQuantity}.");
            }

            // ToQueryString() генерирует SQL запрос из IQueryable
            var generatedSql = dbContext.Courses.ToQueryString();

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
    }
}
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EfCoreBasic_005.Часть_14.Group_by
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
                IsFree = false,
                LessonsQuantity = 7
            };

            var efCoreCourse = new Course
            {
                Name = "Entity Framework Core Basic",
                IsFree = false,
                LessonsQuantity = 10,
            };

            var unitTestsCourse = new Course
            {
                Name = "Юнит-тестирование",
                IsFree = true,
                LessonsQuantity = 3,
            };

            dbContext.Add(csharpCourse);
            dbContext.Add(efCoreCourse);
            dbContext.Add(unitTestsCourse);

            dbContext.SaveChanges();
        }

        public static void Чтение_данных()
        {
            using var dbContext = new ApplicationDbContext();

            var groupedCoursesQueryable = dbContext.Courses
                // группируем курсы по признаку бесплатный/платный 
                .GroupBy(x => x.IsFree)
                // для каждой группы (а их всего две)
                // считаем суммарное количество уроков внутри этих груп
                .Select(
                    x => new
                    {
                        IsFree = x.Key, TotalLessonsQuantity = x.Sum(y => y.LessonsQuantity)
                    });

            var groupedCourses = groupedCoursesQueryable.ToList();

            Console.WriteLine(
                new string(
                    '-',
                    80));
            Console.WriteLine("Информация о курсах.");
            foreach (var groupCourse in groupedCourses)
            {
                Console.WriteLine(
                    groupCourse.IsFree
                        ? "Бесплатные курсы. "
                        : "Платные курсы. " +
                          $"Суммарное количество уроков: {groupCourse.TotalLessonsQuantity}");
            }

            var generatedSql = groupedCoursesQueryable.ToQueryString();

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

        // бесплатный курс или нет
        public bool IsFree { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EfCoreBasic_003.Часть_14.One_to_many
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

            var johnSmith = new Author
            {
                FirstName = "John",
                LastName = "Smith"
            };

            var arthurMorgan = new Author
            {
                FirstName = "Arthur",
                LastName = "Morgan"
            };

            csharpCourse.Author = johnSmith;
            efCoreCourse.Author = johnSmith;

            dbContext.Add(csharpCourse);
            dbContext.Add(efCoreCourse);
            dbContext.Add(arthurMorgan);

            dbContext.SaveChanges();
        }

        public static void Чтение_данных()
        {
            using var dbContext = new ApplicationDbContext();

            var courses = dbContext
                .Courses
                // детальнее метод Include будет рассмотрен в уроке 6
                // сейчас он просто используется в демонстрационных целях для загрузки авторов
                .Include(x => x.Author)
                .ToList();

            Console.WriteLine(
                new string(
                    '-',
                    80));
            Console.WriteLine("Информация о курсах.");
            foreach (var course in courses)
            {
                Console.WriteLine(
                    $"Имя курса: {course.Name}. " +
                    $"Идентификатор курса: {course.Id}. " +
                    $"Количество уроков: {course.LessonsQuantity}. " +
                    $"Автор: {course.Author.FirstName + " " + course.Author.LastName}.");
            }

            Console.WriteLine(
                new string(
                    '-',
                    80));

            var authors = dbContext
                .Authors
                .ToList();

            Console.WriteLine(
                new string(
                    '-',
                    80));
            Console.WriteLine("Информация об авторах.");
            foreach (var author in authors)
            {
                Console.WriteLine($"Автор: {author.FirstName + " " + author.LastName}");
            }

            Console.WriteLine(
                new string(
                    '-',
                    80));
        }
    }

    public class ApplicationDbContext : DbContext
    {
        public DbSet<Course> Courses { get; set; }

        public DbSet<Author> Authors { get; set; }

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
                .Entity<Author>()
                // настраиваем то, что у автора есть множество курсов
                .HasMany(t => t.Courses)
                // настраиваем то, что у курса есть один автор
                .WithOne(t => t.Author)
                // задаем внешний ключ курсов
                .HasForeignKey(t => t.AuthorId)
                // указываем главный ключ в сущности Автор
                .HasPrincipalKey(t => t.Id);
        }
    }

    public class Course
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int LessonsQuantity { get; set; }

        public int AuthorId { get; set; }

        // у курса есть автор
        public Author Author { get; set; }
    }

    public class Author
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        // у автора есть множество курсов
        public ICollection<Course> Courses { get; set; }
    }
}
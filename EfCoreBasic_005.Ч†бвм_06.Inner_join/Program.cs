using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EfCoreBasic_005.Часть_06.Inner_join
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

            var joinedCoursesQueryable = dbContext
                .Courses
                .Join(
                    // объединяем курсы и авторов
                    dbContext.Authors,
                    // ключ из модели курсов
                    x => x.AuthorId,
                    // ключ из модели авторов
                    x => x.Id,
                    // создаём новый объект результат
                    (course, author) => new { Course = course, Author = author });

            var joinedCourses = joinedCoursesQueryable.ToList();

            Console.WriteLine(
                new string(
                    '-',
                    80));
            Console.WriteLine("Информация о курсах.");
            foreach (var joinedCourse in joinedCourses)
            {
                Console.WriteLine(
                    $"Имя курса: {joinedCourse.Course.Name}. " +
                    $"Идентификатор курса: {joinedCourse.Course.Id}. " +
                    $"Количество уроков: {joinedCourse.Course.LessonsQuantity}. " +
                    $"Автор: {joinedCourse.Author.FirstName + " " + joinedCourse.Author.LastName}.");
            }

            var generatedSql = joinedCoursesQueryable.ToQueryString();

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
                .HasMany(t => t.Courses)
                .WithOne(t => t.Author)
                .HasForeignKey(t => t.AuthorId)
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
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EfCoreBasic_003.Часть_15.Many_to_many
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

            var connections = new[]
            {
                new CourseAuthor
                {
                    Course = csharpCourse,
                    Author = johnSmith,
                },
                new CourseAuthor
                {
                    Course = csharpCourse,
                    Author = arthurMorgan,
                },
                new CourseAuthor
                {
                    Course = efCoreCourse,
                    Author = johnSmith,
                },
            };

            dbContext.Add(csharpCourse);
            dbContext.Add(efCoreCourse);
            dbContext.Add(johnSmith);
            dbContext.Add(arthurMorgan);
            dbContext.AddRange(connections);

            dbContext.SaveChanges();
        }

        public static void Чтение_данных()
        {
            using var dbContext = new ApplicationDbContext();

            var courseAuthors = dbContext
                .Set<CourseAuthor>()
                .Include(x => x.Course)
                .Include(x => x.Author)
                .ToList();

            Console.WriteLine(
                new string(
                    '-',
                    80));
            Console.WriteLine("Информация об авторах курсов.");
            foreach (var courseAuthor in courseAuthors)
            {
                Console.WriteLine(
                    $"Имя курса: {courseAuthor.Course.Name}. " +
                    $"Автор: {courseAuthor.Author.FirstName + " " + courseAuthor.Author.LastName}.");
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
            // с использованием промежуточной сущности
            // получается так, что настраиваем два отношения one-to-many
            // от одного курса к множеству промежуточных сущностей
            // от одного автора к множеству промежуточных сущностей

            // очень важно указать здесь составной ключ
            // иначе EF Core не поймёт что требуется конфигурация связи между сущностями как many-to-many
            modelBuilder
                .Entity<CourseAuthor>()
                .HasKey(t => new { t.CourseId, t.AuthorId });

            modelBuilder
                .Entity<Author>()
                // настраиваем то, что у автора есть множество промежуточных сущностей
                .HasMany(t => t.CourseAuthors)
                // настраиваем то, что у промежуточной сущности есть один автор
                .WithOne(t => t.Author)
                // задаем внешний ключ промежуточной сущности
                .HasForeignKey(t => t.AuthorId)
                // указываем главный ключ в сущности Автор
                .HasPrincipalKey(t => t.Id);

            modelBuilder
                .Entity<Course>()
                // настраиваем то, что у автора есть множество промежуточных сущностей
                .HasMany(t => t.CourseAuthors)
                // настраиваем то, что у промежуточной сущности есть один курс
                .WithOne(t => t.Course)
                // задаем внешний ключ промежуточной сущности
                .HasForeignKey(t => t.CourseId)
                // указываем главный ключ в сущности Курс
                .HasPrincipalKey(t => t.Id);
        }
    }

    public class Course
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int LessonsQuantity { get; set; }

        // у курса есть множество авторов
        public ICollection<CourseAuthor> CourseAuthors { get; set; }
    }

    public class Author
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        // у автора есть множество курсов
        public ICollection<CourseAuthor> CourseAuthors { get; set; }
    }

    // специальная промежуточная модель для отношения many-to-many
    public class CourseAuthor
    {
        public int CourseId { get; set; }
        public Course Course { get; set; }

        public int AuthorId { get; set; }
        public Author Author { get; set; }
    }
}
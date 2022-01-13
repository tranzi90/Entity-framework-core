using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EfCoreBasic_003.Часть_07.Последовательности
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

            for (int i = 0; i < 10; i++)
            {
                dbContext.Courses.Add(new Course());
            }

            for (int i = 0; i < 5; i++)
            {
                dbContext.Authors.Add(new Author());
            }

            for (int i = 0; i < 10; i++)
            {
                dbContext.Courses.Add(new Course());
            }

            dbContext.SaveChanges();
        }

        public static void Чтение_данных()
        {
            using var dbContext = new ApplicationDbContext();

            var courses = dbContext.Courses.ToList();

            Console.WriteLine("Курсы:");
            foreach (var course in courses)
            {
                Console.WriteLine($"Идентификатор: {course.Id}. Номер: {course.Number}.");
            }

            Console.WriteLine(
                new string(
                    '-',
                    80));

            var authors = dbContext.Authors.ToList();

            Console.WriteLine("Авторы:");
            foreach (var author in authors)
            {
                Console.WriteLine($"Идентификатор: {author.Id}. Номер: {author.Number}.");
            }
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
            const string Имя_последовательности_в_базе_даннных = "ОбщееКоличествоСущностейВБД";

            modelBuilder.HasSequence<int>(Имя_последовательности_в_базе_даннных);

            modelBuilder
                .Entity<Course>()
                .Property(t => t.Number)
                // по умолчанию у данного свойства будет следующее значение последовательности
                .HasDefaultValueSql($"NEXT VALUE FOR [{Имя_последовательности_в_базе_даннных}]");

            modelBuilder
                .Entity<Author>()
                .Property(t => t.Number)
                .HasDefaultValueSql($"NEXT VALUE FOR [{Имя_последовательности_в_базе_даннных}]");
            ;
        }
    }

    public class Course
    {
        public int Id { get; set; }

        public int Number { get; set; }
    }

    public class Author
    {
        public int Id { get; set; }

        public int Number { get; set; }
    }
}
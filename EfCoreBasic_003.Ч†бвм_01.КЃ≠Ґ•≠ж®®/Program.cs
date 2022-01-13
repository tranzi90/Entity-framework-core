using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EfCoreBasic_003.Часть_01.Конвенции
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Создание_пустой_базы_данных();
        }

        public static void Создание_пустой_базы_данных()
        {
            using var dbContext = new ApplicationDbContext();

            dbContext.Database.EnsureDeleted();

            dbContext.Database.EnsureCreated();
        }
    }

    public class ApplicationDbContext : DbContext
    {
        public DbSet<Course> Courses { get; set; } // по конвенции название таблицы будет Courses

        public DbSet<Author> Authors { get; set; } // по конвенции название таблицы будет Authors

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseSqlServer(
                    @"Server=(localdb)\mssqllocaldb;Database=EfCoreBasicDb;Trusted_Connection=True;")
                .EnableDetailedErrors()
                .EnableSensitiveDataLogging()
                .LogTo(
                    Console.WriteLine,
                    LogLevel.Information);
        }
    }

    // модель курса
    public class Course
    {
        public int Id { get; set; }

        public string Name { get; set; }

        // количество уроков в курсе
        public int LessonsQuantity { get; set; }

        // когда был создан курс
        public DateTimeOffset CreatedAt { get; set; }

        // у курса есть автор
        public Author Author { get; set; }
    }

    // модель автора
    public class Author
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public int? Age { get; set; } // автор может опционально указать свой возраст

        // у автора есть множество курсов
        public ICollection<Course> Courses { get; set; }
    }
}
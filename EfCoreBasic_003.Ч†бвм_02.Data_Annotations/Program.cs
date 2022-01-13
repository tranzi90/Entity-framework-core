using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EfCoreBasic_003.Часть_02.Data_Annotations
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
                    LogLevel.Information);
        }
    }

    // модель курса
    [Table("MyCourses")]
    public class Course
    {
        [Key] // первичный ключ сущности
        public int Id { get; set; }

        [Required] // теперь эта колонка обязательная и не может быть NULL
        [MinLength(10)]
        [MaxLength(500)]
        [Column("MyName")] // имя колонки
        public string Name { get; set; }

        // количество уроков в курсе
        [Column("MyLessonsQuantity")] // имя колонки
        public int LessonsQuantity { get; set; }

        // когда был создан курс
        [Column("MyCreatedAt")] // имя колонки
        public DateTimeOffset CreatedAt { get; set; }

        [Column(
            "MyPrice",
            TypeName = "money")] // имя колонки и тип money
        public decimal Price { get; set; }

        public int AuthorId { get; set; }

        // у курса есть автор
        [ForeignKey("AuthorId")] // указываем на то, что
        // свойство Author использует свойство AuthorId в качестве внешнего ключа
        public Author Author { get; set; }
    }

    // модель автора
    [Table("MyAuthors")]
    public class Author
    {
        [Key] public int Id { get; set; }

        [Required] // теперь эта колонка обязательная и не может быть NULL
        [Column("MyFirstName")]
        public string FirstName { get; set; }

        [Required] // теперь эта колонка обязательная и не может быть NULL
        [Column("MyLastName")]
        public string LastName { get; set; }

        [Column("MyAge")]
        public int? Age { get; set; } // автор может опционально указать свой возраст

        // у автора есть множество курсов
        public ICollection<Course> Courses { get; set; }
    }
}
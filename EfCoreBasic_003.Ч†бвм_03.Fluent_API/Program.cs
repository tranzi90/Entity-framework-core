using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EfCoreBasic_003.Часть_03.Fluent_API
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
        // EF Core по конвенции определяет те модели, которые были явно указаны как DbSet свойства в DbContext
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

        // здесь мы настраиваем моделирование сущностей в EF Core
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // обратите внимание что здесь используется шаблон Строитель
            // https://refactoring.guru/ru/design-patterns/builder
            // это позволяет очень наглядно смоделировать сущность
            modelBuilder
                .Entity<Course>()
                .ToTable("MyCourses");

            modelBuilder
                .Entity<Course>()
                .HasKey(x => x.Id);

            modelBuilder
                .Entity<Course>()
                .Property(x => x.Name)
                .HasColumnName("MyName")
                .HasMaxLength(500)
                .IsRequired();

            modelBuilder
                .Entity<Course>()
                .Property(x => x.LessonsQuantity)
                .HasColumnName("MyLessonsQuantity");

            modelBuilder
                .Entity<Course>()
                .Property(x => x.CreatedAt)
                .HasColumnName("MyCreatedAt");

            modelBuilder
                .Entity<Course>()
                .Property(x => x.Price)
                .HasColumnName("MyPrice")
                .HasColumnType("money");

            modelBuilder
                .Entity<Course>()
                .HasOne(x => x.Author)
                .WithMany(x => x.Courses)
                .IsRequired();

            modelBuilder
                .Entity<Author>()
                .ToTable("MyAuthors");

            modelBuilder
                .Entity<Author>()
                .HasKey(x => x.Id);

            modelBuilder
                .Entity<Author>()
                .Property(x => x.FirstName)
                .HasColumnName("MyFirstName")
                .IsRequired();

            modelBuilder
                .Entity<Author>()
                .Property(x => x.LastName)
                .HasColumnName("MyLastName")
                .IsRequired();

            modelBuilder
                .Entity<Author>()
                .Property(x => x.Age)
                .HasColumnName("MyAge");
        }
    }

    public class Course
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int LessonsQuantity { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public decimal Price { get; set; }

        public Author Author { get; set; }
    }

    public class Author
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public int? Age { get; set; }

        public ICollection<Course> Courses { get; set; }
    }
}
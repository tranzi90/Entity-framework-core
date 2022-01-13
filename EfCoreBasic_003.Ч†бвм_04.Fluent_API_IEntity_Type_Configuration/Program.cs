using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Logging;

namespace EfCoreBasic_003.Часть_04.Fluent_API_IEntity_Type_Configuration
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // автоматически применяем все IEntityTypeConfiguration из текущей сборки
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
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

    public class CourseEntityTypeConfiguration : IEntityTypeConfiguration<Course>
    {
        // метод Configure аналогичен вызову modelBuilder.Entity<Course> из OnModelCreating
        public void Configure(EntityTypeBuilder<Course> builder)
        {
            builder
                .ToTable("MyCourses");

            builder
                .HasKey(x => x.Id);

            builder
                .Property(x => x.Name)
                .HasColumnName("MyName")
                .HasMaxLength(500)
                .IsRequired();

            builder
                .Property(x => x.LessonsQuantity)
                .HasColumnName("MyLessonsQuantity");

            builder
                .Property(x => x.CreatedAt)
                .HasColumnName("MyCreatedAt");

            builder
                .Property(x => x.Price)
                .HasColumnName("MyPrice")
                .HasColumnType("money");

            builder
                .HasOne(x => x.Author)
                .WithMany(x => x.Courses)
                .IsRequired();
        }
    }

    public class Author
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public int? Age { get; set; }

        public ICollection<Course> Courses { get; set; }
    }

    public class AuthorEntityTypeConfiguration : IEntityTypeConfiguration<Author>
    {
        public void Configure(EntityTypeBuilder<Author> builder)
        {
            builder
                .ToTable("MyAuthors");

            builder
                .HasKey(x => x.Id);

            builder
                .Property(x => x.FirstName)
                .HasColumnName("MyFirstName")
                .IsRequired();

            builder
                .Property(x => x.LastName)
                .HasColumnName("MyLastName")
                .IsRequired();

            builder
                .Property(x => x.Age)
                .HasColumnName("MyAge");
        }
    }
}
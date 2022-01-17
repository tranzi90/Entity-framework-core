using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace EfCoreBasic_010.Часть_01.Миграции
{
    public class Program
    {
        public static void Main(string[] args)
        {
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<CourseAuthor>()
                .HasKey(t => new { t.CourseId, t.AuthorId });

            modelBuilder
                .Entity<Author>()
                .HasMany(t => t.CourseAuthors)
                .WithOne(t => t.Author)
                .HasForeignKey(t => t.AuthorId)
                .HasPrincipalKey(t => t.Id);

            modelBuilder
                .Entity<Course>()
                .HasMany(t => t.CourseAuthors)
                .WithOne(t => t.Course)
                .HasForeignKey(t => t.CourseId)
                .HasPrincipalKey(t => t.Id);

            var csharpCourse = new Course
            {
                // заметьте, что необходимо явно указывать идентификатор
                // даже если он генерируется на стороне БД
                Id = 1,
                Name = "C# Advanced",
                LessonsQuantity = 7
            };

            var efCoreCourse = new Course
            {
                Id = 2,
                Name = "Entity Framework Core Basic",
                LessonsQuantity = 10,
            };

            var johnSmith = new Author
            {
                Id = 1,
                FirstName = "John",
                LastName = "Smith"
            };

            var arthurMorgan = new Author
            {
                Id = 2,
                FirstName = "Arthur",
                LastName = "Morgan"
            };

            var connections = new[]
            {
                new CourseAuthor
                {
                    CourseId = 1,
                    AuthorId = 1,
                },
                new CourseAuthor
                {
                    CourseId = 1,
                    AuthorId = 2,
                },
                new CourseAuthor
                {
                    CourseId = 2,
                    AuthorId = 1,
                },
            };

            // наполняем миграцию данными
            modelBuilder
                .Entity<Course>()
                .HasData(
                    csharpCourse,
                    efCoreCourse);

            modelBuilder
                .Entity<Author>()
                .HasData(
                    johnSmith,
                    arthurMorgan);

            modelBuilder
                .Entity<CourseAuthor>()
                .HasData(connections);
        }
    }

    public class Course
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int LessonsQuantity { get; set; }

        public ICollection<CourseAuthor> CourseAuthors { get; set; }
    }

    public class Author
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public ICollection<CourseAuthor> CourseAuthors { get; set; }
    }

    public class CourseAuthor
    {
        public int CourseId { get; set; }
        public Course Course { get; set; }

        public int AuthorId { get; set; }
        public Author Author { get; set; }
    }
}
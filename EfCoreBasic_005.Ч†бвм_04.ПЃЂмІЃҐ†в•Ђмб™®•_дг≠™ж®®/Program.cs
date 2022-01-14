using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Logging;

namespace EfCoreBasic_005.Часть_04.Пользовательские_функции
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

            const string toTicksSql = @"
                CREATE FUNCTION dbo.ToTicks (@DateTime datetime2)
                RETURNS bigint
                AS
                BEGIN
                    RETURN DATEDIFF_BIG( microsecond, '00010101', @DateTime ) * 10 +
                           ( DATEPART( NANOSECOND, @DateTime ) % 1000 ) / 100;
                END";

            const string timeSpanStringToTicksSql = @"
                CREATE FUNCTION dbo.TimeSpanStringToTicks(@timeSpan nvarchar(48))
                RETURNS bigint
                AS
                BEGIN
                    DECLARE @ret bigint
                    DECLARE @defaultYearTicks bigint = dbo.ToTicks(cast('1900-01-01' as datetime2))
                    SELECT @ret = dbo.ToTicks(cast(@timeSpan as datetime2))
                    RETURN @ret - @defaultYearTicks
                END";

            // просто выполняем соответствующие команды на стороне БД
            dbContext.Database.ExecuteSqlRaw(toTicksSql);
            dbContext.Database.ExecuteSqlRaw(timeSpanStringToTicksSql);
        }

        public static void Добавление_данных()
        {
            using var dbContext = new ApplicationDbContext();

            var csharpCourse = new Course
            {
                Name = "C# Advanced",
                LessonsQuantity = 7,
                Duration = TimeSpan.FromHours(7),
            };

            var efCoreCourse = new Course
            {
                Name = "Entity Framework Core Basic",
                LessonsQuantity = 10,
                Duration = TimeSpan.FromHours(10),
            };

            var unitTestsCourse = new Course
            {
                Name = "Юнит-тестирование",
                LessonsQuantity = 3,
                Duration = TimeSpan.FromHours(3)
            };

            dbContext.Add(csharpCourse);
            dbContext.Add(efCoreCourse);
            dbContext.Add(unitTestsCourse);

            dbContext.SaveChanges();
        }

        public static void Чтение_данных()
        {
            using var dbContext = new ApplicationDbContext();

            var totalDuration = dbContext
                .Courses
                // используем пользовательскую функцию
                // также здесь используется доступ к теневому свойству длительности
                // так как сигнатуры свойства и параметра функции не совпадают
                .Sum(
                    x => dbContext.TimeSpanStringToTicks(
                        EF.Property<string>(
                            x,
                            nameof(Course.Duration))));

            Console.WriteLine(
                new string(
                    '-',
                    80));
            Console.WriteLine($"Общая длительность всех курсов: {totalDuration}.");
        }
    }

    public class ApplicationDbContext : DbContext
    {
        public DbSet<Course> Courses { get; set; }

        // конвертирует таймспан в виде строки в тики
        public long TimeSpanStringToTicks(string timeSpan)
            => throw new NotSupportedException();

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
                // явно задаём отношение между пользовательской функцией на стороне EF Core и функцией в БД
                .HasDbFunction(
                    typeof(ApplicationDbContext).GetMethod(
                        // имя пользовательской функции на стороне EF Core
                        nameof(TimeSpanStringToTicks),
                        // массив типов параметров пользовательской функции на стороне EF Core
                        new[] { typeof(string) }))
                // имя пользовательской функции в БД
                .HasName(nameof(TimeSpanStringToTicks));

            modelBuilder
                .Entity<Course>()
                .Property(x => x.Duration)
                // теперь в БД длительность хранится как строка
                // сделано специально для демонстрационных целей
                .HasConversion(new TimeSpanToStringConverter());

            base.OnModelCreating(modelBuilder);
        }
    }

    public class Course
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int LessonsQuantity { get; set; }

        public TimeSpan Duration { get; set; }
    }
}
using System;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EfCoreBasic_002.Часть_2.Строки_подключения
{
    public class Program
    {
        public static void Main(string[] args)
        {
            using var dbContext = new ApplicationDbContext();

            dbContext.Database.ExecuteSqlRaw("SELECT 1");

            Console.WriteLine();
            Console.WriteLine($"Имя провайдера БД: {dbContext.Database.ProviderName}.");
            Console.WriteLine();
        }
    }

    public class ApplicationDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionStringBuilder = new SqlConnectionStringBuilder
            {
                ["Server"] = @"(localdb)\mssqllocaldb", // адрес БД к которой подключаемся
                ["Database"] = "EfCoreBasicDb", // имя БД
                ["Trusted_Connection"] = true // используем аутентификацию Windows
            };

            Console.WriteLine(connectionStringBuilder.ConnectionString);

            // подключаемся к MS SQL Server БД, используя указанную строку подключения
            optionsBuilder
                // настраивает DbContext для подключения к MS SQL Server БД
                .UseSqlServer(connectionStringBuilder.ConnectionString)
                .EnableDetailedErrors()
                .EnableSensitiveDataLogging()
                .LogTo(
                    Console.WriteLine,
                    new[] { DbLoggerCategory.Database.Command.Name },
                    LogLevel.Information);
        }
    }
}
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EfCoreBasic_002.Часть_1.Подключение_к_базе_данных
{
    public class Program
    {
        public static void Main(string[] args)
        {
            using var dbContext = new ApplicationDbContext();

            // команда которая ничего не делает
            // но тем не менее она выполняется на стороне БД
            // убеждаемся в том, что мы действительно открыли соединение с БД
            dbContext.Database.ExecuteSqlRaw("SELECT 1");

            Console.WriteLine();
            Console.WriteLine($"Имя провайдера БД: {dbContext.Database.ProviderName}.");
            Console.WriteLine();
        }
    }

    // абстракция подключения к БД
    public class ApplicationDbContext : DbContext
    {
        // метод конфигурации подключения к БД
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // подключаемся к MS SQL Server БД, используя указанную строку подключения
            optionsBuilder
                // настраивает DbContext для подключения к MS SQL Server БД
                .UseSqlServer(
                    @"Server=(localdb)\mssqllocaldb;Database=EfCoreBasicDb;Trusted_Connection=True;")
                // включает более детальный вывод ошибок самого EF Core
                .EnableDetailedErrors()
                // включает вывод приватных данных приложения (таких как сгенерированные строки запроса, параметры этих строк запроса)
                .EnableSensitiveDataLogging()
                // логируем всё в консоль
                // также дополнительно отфильтровываем логи, оставляем только запросы в БД
                .LogTo(
                    Console.WriteLine,
                    new[] { DbLoggerCategory.Database.Command.Name },
                    LogLevel.Information);
        }
    }
}
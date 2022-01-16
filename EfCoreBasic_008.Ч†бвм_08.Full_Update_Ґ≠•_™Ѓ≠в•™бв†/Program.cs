﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace EfCoreBasic_008.Часть_08.Full_Update_вне_контекста
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Создание_пустой_базы_данных();
            Добавление_данных();
            Обновление_сущности();
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

            csharpCourse.Author = johnSmith;
            efCoreCourse.Author = arthurMorgan;

            dbContext.Add(csharpCourse);
            dbContext.Add(efCoreCourse);
            dbContext.Add(arthurMorgan);

            dbContext.SaveChanges();
        }

        public static void Обновление_сущности()
        {
            using var dbContext = new ApplicationDbContext();

            var author = new Author
            {
                Id = 2,
                FirstName = "Jessy (was Arthur)",
                LastName = "Brown (was Morgan)"
            };

            Console.WriteLine(
                new string(
                    '-',
                    80));
            Console.WriteLine(
                dbContext.ChangeTracker.DebugView.LongView == ""
                    ? "Контекст пустой."
                    : dbContext.ChangeTracker.DebugView.LongView);

            dbContext.Update(author);

            Console.WriteLine(
                new string(
                    '-',
                    80));
            Console.WriteLine(dbContext.ChangeTracker.DebugView.LongView);

            dbContext.SaveChanges();

            Console.WriteLine(
                new string(
                    '-',
                    80));
            Console.WriteLine(dbContext.ChangeTracker.DebugView.LongView);
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
                    @"Server=(localdb)\mssqllocaldb;Database=EfCoreBasicDb;Trusted_Connection=True;");
        }
    }

    public class Course
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int LessonsQuantity { get; set; }

        public Author Author { get; set; }
    }

    public class Author
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public ICollection<Course> Courses { get; set; }
    }
}
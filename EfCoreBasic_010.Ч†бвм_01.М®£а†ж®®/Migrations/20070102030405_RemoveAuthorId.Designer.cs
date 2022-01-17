﻿// <auto-generated />
using EfCoreBasic_010.Часть_01.Миграции;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EfCoreBasic_010.Часть_01.Миграции.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20070102030405_RemoveAuthorId")]
    partial class RemoveAuthorId
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.11")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("EfCoreBasic_010.Часть_01.Миграции.Author", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("FirstName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Author");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            FirstName = "John",
                            LastName = "Smith"
                        },
                        new
                        {
                            Id = 2,
                            FirstName = "Arthur",
                            LastName = "Morgan"
                        });
                });

            modelBuilder.Entity("EfCoreBasic_010.Часть_01.Миграции.Course", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("LessonsQuantity")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Courses");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            LessonsQuantity = 7,
                            Name = "C# Advanced"
                        },
                        new
                        {
                            Id = 2,
                            LessonsQuantity = 10,
                            Name = "Entity Framework Core Basic"
                        });
                });

            modelBuilder.Entity("EfCoreBasic_010.Часть_01.Миграции.CourseAuthor", b =>
                {
                    b.Property<int>("CourseId")
                        .HasColumnType("int");

                    b.Property<int>("AuthorId")
                        .HasColumnType("int");

                    b.HasKey("CourseId", "AuthorId");

                    b.HasIndex("AuthorId");

                    b.ToTable("CourseAuthor");

                    b.HasData(
                        new
                        {
                            CourseId = 1,
                            AuthorId = 1
                        },
                        new
                        {
                            CourseId = 1,
                            AuthorId = 2
                        },
                        new
                        {
                            CourseId = 2,
                            AuthorId = 1
                        });
                });

            modelBuilder.Entity("EfCoreBasic_010.Часть_01.Миграции.CourseAuthor", b =>
                {
                    b.HasOne("EfCoreBasic_010.Часть_01.Миграции.Author", "Author")
                        .WithMany("CourseAuthors")
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("EfCoreBasic_010.Часть_01.Миграции.Course", "Course")
                        .WithMany("CourseAuthors")
                        .HasForeignKey("CourseId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Author");

                    b.Navigation("Course");
                });

            modelBuilder.Entity("EfCoreBasic_010.Часть_01.Миграции.Author", b =>
                {
                    b.Navigation("CourseAuthors");
                });

            modelBuilder.Entity("EfCoreBasic_010.Часть_01.Миграции.Course", b =>
                {
                    b.Navigation("CourseAuthors");
                });
#pragma warning restore 612, 618
        }
    }
}

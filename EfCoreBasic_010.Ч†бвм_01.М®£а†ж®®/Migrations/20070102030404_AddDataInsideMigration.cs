using Microsoft.EntityFrameworkCore.Migrations;

namespace EfCoreBasic_010.Часть_01.Миграции.Migrations
{
    public partial class AddDataInsideMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Author",
                columns: new[] { "Id", "FirstName", "LastName" },
                values: new object[,]
                {
                    { 1, "John", "Smith" },
                    { 2, "Arthur", "Morgan" }
                });

            migrationBuilder.InsertData(
                table: "Courses",
                columns: new[] { "Id", "AuthorId", "LessonsQuantity", "Name" },
                values: new object[,]
                {
                    { 1, 0, 7, "C# Advanced" },
                    { 2, 0, 10, "Entity Framework Core Basic" }
                });

            migrationBuilder.InsertData(
                table: "CourseAuthor",
                columns: new[] { "AuthorId", "CourseId" },
                values: new object[] { 1, 1 });

            migrationBuilder.InsertData(
                table: "CourseAuthor",
                columns: new[] { "AuthorId", "CourseId" },
                values: new object[] { 2, 1 });

            migrationBuilder.InsertData(
                table: "CourseAuthor",
                columns: new[] { "AuthorId", "CourseId" },
                values: new object[] { 1, 2 });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "CourseAuthor",
                keyColumns: new[] { "AuthorId", "CourseId" },
                keyValues: new object[] { 1, 1 });

            migrationBuilder.DeleteData(
                table: "CourseAuthor",
                keyColumns: new[] { "AuthorId", "CourseId" },
                keyValues: new object[] { 2, 1 });

            migrationBuilder.DeleteData(
                table: "CourseAuthor",
                keyColumns: new[] { "AuthorId", "CourseId" },
                keyValues: new object[] { 1, 2 });

            migrationBuilder.DeleteData(
                table: "Author",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Author",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}

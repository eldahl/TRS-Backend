using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TRS_backend.Migrations
{
    /// <inheritdoc />
    public partial class InitialUserTableWithDataSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Email = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    Username = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false),
                    PasswordHash = table.Column<byte[]>(type: "varbinary(1024)", maxLength: 1024, nullable: false),
                    Salt = table.Column<byte[]>(type: "varbinary(1024)", maxLength: 1024, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "PasswordHash", "Role", "Salt", "Username" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 5, 16, 0, 31, 23, 525, DateTimeKind.Local).AddTicks(1852), "testAdmin@test.dk", new byte[] { 132, 127, 146, 203, 232, 109, 168, 134, 95, 67, 76, 196, 80, 72, 249, 242, 248, 3, 2, 67, 173, 180, 13, 74, 95, 31, 117, 172, 196, 200, 86, 2 }, 0, new byte[] { 227, 9, 16, 1, 143, 59, 46, 90, 127, 226, 72, 61, 178, 219, 241, 179, 4, 47, 198, 132, 217, 132, 18, 97, 251, 195, 96, 207, 27, 150, 58, 202 }, "testAdmin" },
                    { 2, new DateTime(2024, 5, 16, 0, 31, 23, 525, DateTimeKind.Local).AddTicks(1906), "testUser@test.dk", new byte[] { 254, 32, 60, 208, 50, 84, 131, 38, 59, 59, 98, 231, 90, 130, 11, 104, 235, 163, 209, 117, 14, 63, 250, 105, 243, 9, 0, 101, 242, 252, 92, 20 }, 1, new byte[] { 74, 156, 203, 196, 206, 6, 47, 163, 200, 212, 177, 245, 12, 138, 51, 21, 9, 101, 165, 37, 158, 222, 148, 43, 151, 217, 222, 94, 146, 65, 142, 232 }, "testUser" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}

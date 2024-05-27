using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TRS_backend.Migrations
{
    /// <inheritdoc />
    public partial class DatabaseSetup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "OpenDays",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    OpenTime = table.Column<TimeSpan>(type: "time(6)", nullable: false),
                    CloseTime = table.Column<TimeSpan>(type: "time(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpenDays", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "StaffNotificationEmails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Email = table.Column<string>(type: "longtext", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaffNotificationEmails", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "StaffNotificationPhoneNumbers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    CountryCode = table.Column<int>(type: "int", nullable: false),
                    PhoneNumber = table.Column<string>(type: "longtext", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaffNotificationPhoneNumbers", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Tables",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    TableName = table.Column<string>(type: "longtext", nullable: false),
                    Seats = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tables", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TimeSlots",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "time(6)", nullable: false),
                    Duration = table.Column<TimeSpan>(type: "time(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeSlots", x => x.Id);
                })
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

            migrationBuilder.CreateTable(
                name: "TableReservations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    TableId = table.Column<int>(type: "int", nullable: false),
                    OpenDayId = table.Column<int>(type: "int", nullable: false),
                    TimeSlotId = table.Column<int>(type: "int", nullable: false),
                    FullName = table.Column<string>(type: "longtext", nullable: false),
                    Email = table.Column<string>(type: "longtext", nullable: false),
                    PhoneNumber = table.Column<string>(type: "longtext", nullable: false),
                    SendReminders = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Comment = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TableReservations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TableReservations_OpenDays_OpenDayId",
                        column: x => x.OpenDayId,
                        principalTable: "OpenDays",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TableReservations_Tables_TableId",
                        column: x => x.TableId,
                        principalTable: "Tables",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TableReservations_TimeSlots_TimeSlotId",
                        column: x => x.TimeSlotId,
                        principalTable: "TimeSlots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.InsertData(
                table: "OpenDays",
                columns: new[] { "Id", "CloseTime", "Date", "OpenTime" },
                values: new object[,]
                {
                    { 1, new TimeSpan(0, 21, 0, 0, 0), new DateTime(2024, 5, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 16, 0, 0, 0) },
                    { 2, new TimeSpan(0, 21, 0, 0, 0), new DateTime(2024, 5, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 16, 0, 0, 0) }
                });

            migrationBuilder.InsertData(
                table: "Tables",
                columns: new[] { "Id", "Seats", "TableName" },
                values: new object[,]
                {
                    { 1, 8, "Table 10" },
                    { 2, 6, "Table 11" },
                    { 3, 6, "Table 12" },
                    { 4, 4, "Table 13" },
                    { 5, 4, "Table 17" },
                    { 6, 4, "Table 19" },
                    { 7, 8, "Table 101" },
                    { 8, 8, "Table 110" },
                    { 9, 6, "Table 103" }
                });

            migrationBuilder.InsertData(
                table: "TimeSlots",
                columns: new[] { "Id", "Date", "Duration", "StartTime" },
                values: new object[] { 1, new DateTime(2024, 5, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0), new TimeSpan(0, 16, 0, 0, 0) });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "PasswordHash", "Role", "Salt", "Username" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 5, 27, 5, 37, 12, 640, DateTimeKind.Local).AddTicks(1148), "testAdmin@test.dk", new byte[] { 132, 127, 146, 203, 232, 109, 168, 134, 95, 67, 76, 196, 80, 72, 249, 242, 248, 3, 2, 67, 173, 180, 13, 74, 95, 31, 117, 172, 196, 200, 86, 2 }, 0, new byte[] { 227, 9, 16, 1, 143, 59, 46, 90, 127, 226, 72, 61, 178, 219, 241, 179, 4, 47, 198, 132, 217, 132, 18, 97, 251, 195, 96, 207, 27, 150, 58, 202 }, "testAdmin" },
                    { 2, new DateTime(2024, 5, 27, 5, 37, 12, 640, DateTimeKind.Local).AddTicks(1204), "testUser@test.dk", new byte[] { 254, 32, 60, 208, 50, 84, 131, 38, 59, 59, 98, 231, 90, 130, 11, 104, 235, 163, 209, 117, 14, 63, 250, 105, 243, 9, 0, 101, 242, 252, 92, 20 }, 1, new byte[] { 74, 156, 203, 196, 206, 6, 47, 163, 200, 212, 177, 245, 12, 138, 51, 21, 9, 101, 165, 37, 158, 222, 148, 43, 151, 217, 222, 94, 146, 65, 142, 232 }, "testUser" }
                });

            migrationBuilder.InsertData(
                table: "TableReservations",
                columns: new[] { "Id", "Comment", "Email", "FullName", "OpenDayId", "PhoneNumber", "SendReminders", "TableId", "TimeSlotId" },
                values: new object[] { 1, "This is a comment", "customer@gmail.com", "Customer user", 1, "12345678", true, 1, 1 });

            migrationBuilder.CreateIndex(
                name: "IX_OpenDays_Date",
                table: "OpenDays",
                column: "Date",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TableReservations_OpenDayId",
                table: "TableReservations",
                column: "OpenDayId");

            migrationBuilder.CreateIndex(
                name: "IX_TableReservations_TableId",
                table: "TableReservations",
                column: "TableId");

            migrationBuilder.CreateIndex(
                name: "IX_TableReservations_TimeSlotId",
                table: "TableReservations",
                column: "TimeSlotId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StaffNotificationEmails");

            migrationBuilder.DropTable(
                name: "StaffNotificationPhoneNumbers");

            migrationBuilder.DropTable(
                name: "TableReservations");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "OpenDays");

            migrationBuilder.DropTable(
                name: "Tables");

            migrationBuilder.DropTable(
                name: "TimeSlots");
        }
    }
}

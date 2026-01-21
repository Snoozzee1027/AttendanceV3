using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttendanceV3.Migrations
{
    /// <inheritdoc />
    public partial class InitAttendance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StudentEmail",
                table: "AttendanceSessions");

            migrationBuilder.DropColumn(
                name: "StudentId",
                table: "AttendanceRecords");

            migrationBuilder.AlterColumn<string>(
                name: "IpAddress",
                table: "AttendanceRecords",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddColumn<string>(
                name: "DeviceFingerprint",
                table: "AttendanceRecords",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Students",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    StudentCode = table.Column<string>(type: "TEXT", nullable: false),
                    FullName = table.Column<string>(type: "TEXT", nullable: false),
                    YearLevel = table.Column<string>(type: "TEXT", nullable: false),
                    ClassSection = table.Column<string>(type: "TEXT", nullable: false),
                    StudentId = table.Column<int>(type: "INTEGER", nullable: false),
                    UserName = table.Column<string>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceRecords_StudentCode_AttendanceSessionId",
                table: "AttendanceRecords",
                columns: new[] { "StudentCode", "AttendanceSessionId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Students");

            migrationBuilder.DropIndex(
                name: "IX_AttendanceRecords_StudentCode_AttendanceSessionId",
                table: "AttendanceRecords");

            migrationBuilder.DropColumn(
                name: "DeviceFingerprint",
                table: "AttendanceRecords");

            migrationBuilder.AddColumn<string>(
                name: "StudentEmail",
                table: "AttendanceSessions",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "IpAddress",
                table: "AttendanceRecords",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StudentId",
                table: "AttendanceRecords",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}

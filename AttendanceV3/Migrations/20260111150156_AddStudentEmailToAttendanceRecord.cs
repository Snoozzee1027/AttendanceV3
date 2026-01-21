using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttendanceV3.Migrations
{
    /// <inheritdoc />
    public partial class AddStudentEmailToAttendanceRecord : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StudentEmail",
                table: "AttendanceSessions",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TeacherEmail",
                table: "AttendanceSessions",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "StudentEmail",
                table: "AttendanceRecords",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceRecords_StudentCode_AttendanceSessionId",
                table: "AttendanceRecords",
                columns: new[] { "StudentCode", "AttendanceSessionId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AttendanceRecords_StudentCode_AttendanceSessionId",
                table: "AttendanceRecords");

            migrationBuilder.DropColumn(
                name: "StudentEmail",
                table: "AttendanceSessions");

            migrationBuilder.DropColumn(
                name: "TeacherEmail",
                table: "AttendanceSessions");

            migrationBuilder.DropColumn(
                name: "StudentEmail",
                table: "AttendanceRecords");
        }
    }
}

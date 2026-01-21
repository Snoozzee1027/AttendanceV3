using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttendanceV3.Migrations
{
    /// <inheritdoc />
    public partial class AddStudentNameToAttendanceRecord : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StudentName",
                table: "AttendanceRecords",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StudentName",
                table: "AttendanceRecords");
        }
    }
}

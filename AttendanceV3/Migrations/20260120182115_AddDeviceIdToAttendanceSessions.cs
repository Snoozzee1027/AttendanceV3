using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttendanceV3.Migrations
{
    /// <inheritdoc />
    public partial class AddDeviceIdToAttendanceSessions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DeviceId",
                table: "AttendanceSessions",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DeviceId",
                table: "AttendanceRecords",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "SessionId",
                table: "AttendanceRecords",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeviceId",
                table: "AttendanceSessions");

            migrationBuilder.DropColumn(
                name: "DeviceId",
                table: "AttendanceRecords");

            migrationBuilder.DropColumn(
                name: "SessionId",
                table: "AttendanceRecords");
        }
    }
}

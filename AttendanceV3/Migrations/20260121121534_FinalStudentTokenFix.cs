using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttendanceV3.Migrations
{
    /// <inheritdoc />
    public partial class FinalStudentTokenFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StudentToken",
                table: "AttendanceRecords",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StudentToken",
                table: "AttendanceRecords");
        }
    }
}

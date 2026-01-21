using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttendanceV3.Migrations
{
    /// <inheritdoc />
    public partial class AddIpAddressColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IpAddress",
                table: "StudentDevices",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IpAddress",
                table: "StudentDevices");
        }
    }
}

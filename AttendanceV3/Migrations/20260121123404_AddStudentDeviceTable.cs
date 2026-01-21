using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttendanceV3.Migrations
{
    /// <inheritdoc />
    public partial class AddStudentDeviceTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AttendanceRecords_AttendanceSessions_AttendanceSessionId",
                table: "AttendanceRecords");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AttendanceRecords",
                table: "AttendanceRecords");

            migrationBuilder.DropIndex(
                name: "IX_AttendanceRecords_StudentCode_AttendanceSessionId",
                table: "AttendanceRecords");

            migrationBuilder.RenameTable(
                name: "AttendanceRecords",
                newName: "AttendanceRecord");

            migrationBuilder.RenameIndex(
                name: "IX_AttendanceRecords_AttendanceSessionId",
                table: "AttendanceRecord",
                newName: "IX_AttendanceRecord_AttendanceSessionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AttendanceRecord",
                table: "AttendanceRecord",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "StudentDevices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DeviceId = table.Column<string>(type: "TEXT", nullable: false),
                    StudentName = table.Column<string>(type: "TEXT", nullable: false),
                    StudentEmail = table.Column<string>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastLogin = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentDevices", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StudentDevices_DeviceId",
                table: "StudentDevices",
                column: "DeviceId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AttendanceRecord_AttendanceSessions_AttendanceSessionId",
                table: "AttendanceRecord",
                column: "AttendanceSessionId",
                principalTable: "AttendanceSessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AttendanceRecord_AttendanceSessions_AttendanceSessionId",
                table: "AttendanceRecord");

            migrationBuilder.DropTable(
                name: "StudentDevices");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AttendanceRecord",
                table: "AttendanceRecord");

            migrationBuilder.RenameTable(
                name: "AttendanceRecord",
                newName: "AttendanceRecords");

            migrationBuilder.RenameIndex(
                name: "IX_AttendanceRecord_AttendanceSessionId",
                table: "AttendanceRecords",
                newName: "IX_AttendanceRecords_AttendanceSessionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AttendanceRecords",
                table: "AttendanceRecords",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceRecords_StudentCode_AttendanceSessionId",
                table: "AttendanceRecords",
                columns: new[] { "StudentCode", "AttendanceSessionId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AttendanceRecords_AttendanceSessions_AttendanceSessionId",
                table: "AttendanceRecords",
                column: "AttendanceSessionId",
                principalTable: "AttendanceSessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

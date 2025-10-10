using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AllDemo.Migrations
{
    /// <inheritdoc />
    public partial class SemesterModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tblAcademicYear",
                columns: table => new
                {
                    Year_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Year_Name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Start_Date = table.Column<DateOnly>(type: "date", nullable: false),
                    End_Date = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblAcademicYear", x => x.Year_Id);
                });

            migrationBuilder.CreateTable(
                name: "tblSemester",
                columns: table => new
                {
                    SemesterId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Year_Id = table.Column<int>(type: "int", nullable: false),
                    SemesterName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: true),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblSemester", x => x.SemesterId);
                    table.ForeignKey(
                        name: "FK_tblSemester_tblAcademicYear_Year_Id",
                        column: x => x.Year_Id,
                        principalTable: "tblAcademicYear",
                        principalColumn: "Year_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tblSemester_Year_Id",
                table: "tblSemester",
                column: "Year_Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tblSemester");

            migrationBuilder.DropTable(
                name: "tblAcademicYear");
        }
    }
}

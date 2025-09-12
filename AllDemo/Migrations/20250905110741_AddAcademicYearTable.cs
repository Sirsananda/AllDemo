using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AllDemo.Migrations
{
    /// <inheritdoc />
    public partial class AddAcademicYearTable : Migration
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tblAcademicYear");
        }
    }
}

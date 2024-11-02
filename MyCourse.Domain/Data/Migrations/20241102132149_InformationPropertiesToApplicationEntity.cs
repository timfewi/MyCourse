using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyCourse.Domain.Data.Migrations
{
    /// <inheritdoc />
    public partial class InformationPropertiesToApplicationEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Comments",
                table: "Applications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExperienceLevel",
                table: "Applications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PrefferdStyle",
                table: "Applications",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Comments",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "ExperienceLevel",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "PrefferdStyle",
                table: "Applications");
        }
    }
}

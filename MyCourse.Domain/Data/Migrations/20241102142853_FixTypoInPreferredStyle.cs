using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyCourse.Domain.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixTypoInPreferredStyle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PrefferdStyle",
                table: "Applications",
                newName: "PreferredStyle");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PreferredStyle",
                table: "Applications",
                newName: "PrefferdStyle");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyCourse.Domain.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCaptionFromEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Caption",
                table: "BlogPostMedias");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Caption",
                table: "BlogPostMedias",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}

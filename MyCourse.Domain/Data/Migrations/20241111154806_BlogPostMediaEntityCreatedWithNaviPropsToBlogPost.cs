using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyCourse.Domain.Data.Migrations
{
    /// <inheritdoc />
    public partial class BlogPostMediaEntityCreatedWithNaviPropsToBlogPost : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BlogPostMedia_BlogPost_BlogPostId",
                table: "BlogPostMedia");

            migrationBuilder.DropForeignKey(
                name: "FK_BlogPostMedia_Medias_MediaId",
                table: "BlogPostMedia");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BlogPostMedia",
                table: "BlogPostMedia");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BlogPost",
                table: "BlogPost");

            migrationBuilder.RenameTable(
                name: "BlogPostMedia",
                newName: "BlogPostMedias");

            migrationBuilder.RenameTable(
                name: "BlogPost",
                newName: "BlogPosts");

            migrationBuilder.RenameIndex(
                name: "IX_BlogPostMedia_MediaId",
                table: "BlogPostMedias",
                newName: "IX_BlogPostMedias_MediaId");

            migrationBuilder.RenameIndex(
                name: "IX_BlogPostMedia_BlogPostId",
                table: "BlogPostMedias",
                newName: "IX_BlogPostMedias_BlogPostId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BlogPostMedias",
                table: "BlogPostMedias",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BlogPosts",
                table: "BlogPosts",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BlogPostMedias_BlogPosts_BlogPostId",
                table: "BlogPostMedias",
                column: "BlogPostId",
                principalTable: "BlogPosts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BlogPostMedias_Medias_MediaId",
                table: "BlogPostMedias",
                column: "MediaId",
                principalTable: "Medias",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BlogPostMedias_BlogPosts_BlogPostId",
                table: "BlogPostMedias");

            migrationBuilder.DropForeignKey(
                name: "FK_BlogPostMedias_Medias_MediaId",
                table: "BlogPostMedias");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BlogPosts",
                table: "BlogPosts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BlogPostMedias",
                table: "BlogPostMedias");

            migrationBuilder.RenameTable(
                name: "BlogPosts",
                newName: "BlogPost");

            migrationBuilder.RenameTable(
                name: "BlogPostMedias",
                newName: "BlogPostMedia");

            migrationBuilder.RenameIndex(
                name: "IX_BlogPostMedias_MediaId",
                table: "BlogPostMedia",
                newName: "IX_BlogPostMedia_MediaId");

            migrationBuilder.RenameIndex(
                name: "IX_BlogPostMedias_BlogPostId",
                table: "BlogPostMedia",
                newName: "IX_BlogPostMedia_BlogPostId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BlogPost",
                table: "BlogPost",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BlogPostMedia",
                table: "BlogPostMedia",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BlogPostMedia_BlogPost_BlogPostId",
                table: "BlogPostMedia",
                column: "BlogPostId",
                principalTable: "BlogPost",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BlogPostMedia_Medias_MediaId",
                table: "BlogPostMedia",
                column: "MediaId",
                principalTable: "Medias",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

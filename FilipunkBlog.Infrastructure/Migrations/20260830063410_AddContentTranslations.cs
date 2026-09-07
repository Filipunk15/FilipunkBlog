using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FilipunkBlog.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddContentTranslations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NameEn",
                table: "Tags",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AvailabilityNoteEn",
                table: "SiteSettings",
                type: "nvarchar(160)",
                maxLength: 160,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameEn",
                table: "Categories",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BlogPostTranslations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BlogPostId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Culture = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Introduction = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlogPostTranslations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BlogPostTranslations_BlogPosts_BlogPostId",
                        column: x => x.BlogPostId,
                        principalTable: "BlogPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectTranslations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Culture = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Detail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectTranslations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectTranslations_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BlogPostTranslations_BlogPostId_Culture",
                table: "BlogPostTranslations",
                columns: new[] { "BlogPostId", "Culture" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BlogPostTranslations_Culture_Slug",
                table: "BlogPostTranslations",
                columns: new[] { "Culture", "Slug" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTranslations_Culture_Slug",
                table: "ProjectTranslations",
                columns: new[] { "Culture", "Slug" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTranslations_ProjectId_Culture",
                table: "ProjectTranslations",
                columns: new[] { "ProjectId", "Culture" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BlogPostTranslations");

            migrationBuilder.DropTable(
                name: "ProjectTranslations");

            migrationBuilder.DropColumn(
                name: "NameEn",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "AvailabilityNoteEn",
                table: "SiteSettings");

            migrationBuilder.DropColumn(
                name: "NameEn",
                table: "Categories");
        }
    }
}

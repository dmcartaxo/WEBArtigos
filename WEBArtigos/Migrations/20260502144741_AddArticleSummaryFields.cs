using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WEBArtigos.Migrations
{
    /// <inheritdoc />
    public partial class AddArticleSummaryFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OriginalFileName",
                table: "Articles",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Summary",
                table: "Articles",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OriginalFileName",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "Summary",
                table: "Articles");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ForumPlatform.Forum.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFinalScore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "FinalScore",
                table: "Threads",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "FinalScore",
                table: "Comments",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FinalScore",
                table: "Threads");

            migrationBuilder.DropColumn(
                name: "FinalScore",
                table: "Comments");
        }
    }
}

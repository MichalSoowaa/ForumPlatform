using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ForumPlatform.Users.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ColumnNameChange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MeritIndicator",
                schema: "users",
                table: "Users",
                newName: "QualityIndicator");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "QualityIndicator",
                schema: "users",
                table: "Users",
                newName: "MeritIndicator");
        }
    }
}

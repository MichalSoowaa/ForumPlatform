using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ForumPlatform.Users.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DropVersionColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Version",
                schema: "users",
                table: "Users");

            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                schema: "users",
                table: "Users",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValue: 0u);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "xmin",
                schema: "users",
                table: "Users");

            migrationBuilder.AddColumn<byte[]>(
                name: "Version",
                schema: "users",
                table: "Users",
                type: "bytea",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ForumPlatform.Reputation.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddVote : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Votes",
                schema: "reputation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    VoterId = table.Column<Guid>(type: "uuid", nullable: false),
                    ContentId = table.Column<Guid>(type: "uuid", nullable: false),
                    ContentType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Value = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    WeightAtCast = table.Column<decimal>(type: "numeric(5,4)", precision: 5, scale: 4, nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Votes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Votes_ContentId_ContentType",
                schema: "reputation",
                table: "Votes",
                columns: new[] { "ContentId", "ContentType" });

            migrationBuilder.CreateIndex(
                name: "IX_Votes_VoterId_ContentId_ContentType",
                schema: "reputation",
                table: "Votes",
                columns: new[] { "VoterId", "ContentId", "ContentType" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Votes",
                schema: "reputation");
        }
    }
}

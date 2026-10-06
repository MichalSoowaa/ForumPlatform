using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ForumPlatform.Reputation.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "reputation");

            migrationBuilder.CreateTable(
                name: "QualityAssessments",
                schema: "reputation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ContentId = table.Column<Guid>(type: "uuid", nullable: false),
                    ContentType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    HeuristicScore = table.Column<decimal>(type: "numeric(5,4)", precision: 5, scale: 4, nullable: false),
                    LlmVerdict = table.Column<decimal>(type: "numeric(5,4)", precision: 5, scale: 4, nullable: true),
                    LlmJustification = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QualityAssessments", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_QualityAssessments_ContentId_ContentType",
                schema: "reputation",
                table: "QualityAssessments",
                columns: new[] { "ContentId", "ContentType" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QualityAssessments",
                schema: "reputation");
        }
    }
}

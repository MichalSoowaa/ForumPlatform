using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ForumPlatform.Forum.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddThreadVisits : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ThreadVisits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ThreadId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastVisitedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SessionBaselineAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThreadVisits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ThreadVisits_Threads_ThreadId",
                        column: x => x.ThreadId,
                        principalTable: "Threads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ThreadVisits_ThreadId",
                table: "ThreadVisits",
                column: "ThreadId");

            migrationBuilder.CreateIndex(
                name: "IX_ThreadVisits_UserId_ThreadId",
                table: "ThreadVisits",
                columns: new[] { "UserId", "ThreadId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ThreadVisits");
        }
    }
}

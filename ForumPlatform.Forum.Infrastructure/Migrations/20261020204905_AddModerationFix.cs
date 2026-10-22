using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ForumPlatform.Forum.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddModerationFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Threads",
                type: "character varying(300)",
                maxLength: 300,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<decimal>(
                name: "FinalScore",
                table: "Threads",
                type: "numeric(5,4)",
                precision: 5,
                scale: 4,
                nullable: false,
                defaultValue: 0.5m,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<int>(
                name: "CommentCount",
                table: "Threads",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                table: "Threads",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AlterColumn<List<string>>(
                name: "Tags",
                table: "Subforums",
                type: "text[]",
                nullable: false,
                defaultValueSql: "'{}'",
                oldClrType: typeof(List<string>),
                oldType: "text[]");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Subforums",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "RejectionReason",
                table: "Subforums",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Subforums",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Subforums",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                table: "Subforums",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AlterColumn<decimal>(
                name: "FinalScore",
                table: "Comments",
                type: "numeric(5,4)",
                precision: 5,
                scale: 4,
                nullable: false,
                defaultValue: 0.5m,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                table: "Comments",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.CreateIndex(
                name: "IX_Threads_CreatedAt",
                table: "Threads",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Threads_LastActivityAt",
                table: "Threads",
                column: "LastActivityAt");

            migrationBuilder.CreateIndex(
                name: "IX_Threads_SubforumId",
                table: "Threads",
                column: "SubforumId");

            migrationBuilder.CreateIndex(
                name: "IX_Subforums_Name",
                table: "Subforums",
                column: "Name",
                unique: true,
                filter: "\"IsDeleted\" = false AND \"Status\" <> 'Rejected'");

            migrationBuilder.CreateIndex(
                name: "IX_Subforums_Status",
                table: "Subforums",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_ParentCommentId",
                table: "Comments",
                column: "ParentCommentId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_ThreadId",
                table: "Comments",
                column: "ThreadId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Comments_ParentCommentId",
                table: "Comments",
                column: "ParentCommentId",
                principalTable: "Comments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Threads_ThreadId",
                table: "Comments",
                column: "ThreadId",
                principalTable: "Threads",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Threads_Subforums_SubforumId",
                table: "Threads",
                column: "SubforumId",
                principalTable: "Subforums",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Comments_ParentCommentId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Threads_ThreadId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Threads_Subforums_SubforumId",
                table: "Threads");

            migrationBuilder.DropIndex(
                name: "IX_Threads_CreatedAt",
                table: "Threads");

            migrationBuilder.DropIndex(
                name: "IX_Threads_LastActivityAt",
                table: "Threads");

            migrationBuilder.DropIndex(
                name: "IX_Threads_SubforumId",
                table: "Threads");

            migrationBuilder.DropIndex(
                name: "IX_Subforums_Name",
                table: "Subforums");

            migrationBuilder.DropIndex(
                name: "IX_Subforums_Status",
                table: "Subforums");

            migrationBuilder.DropIndex(
                name: "IX_Comments_ParentCommentId",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_Comments_ThreadId",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "xmin",
                table: "Threads");

            migrationBuilder.DropColumn(
                name: "xmin",
                table: "Subforums");

            migrationBuilder.DropColumn(
                name: "xmin",
                table: "Comments");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Threads",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(300)",
                oldMaxLength: 300);

            migrationBuilder.AlterColumn<decimal>(
                name: "FinalScore",
                table: "Threads",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(5,4)",
                oldPrecision: 5,
                oldScale: 4,
                oldDefaultValue: 0.5m);

            migrationBuilder.AlterColumn<int>(
                name: "CommentCount",
                table: "Threads",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 0);

            migrationBuilder.AlterColumn<List<string>>(
                name: "Tags",
                table: "Subforums",
                type: "text[]",
                nullable: false,
                oldClrType: typeof(List<string>),
                oldType: "text[]",
                oldDefaultValueSql: "'{}'");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Subforums",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "RejectionReason",
                table: "Subforums",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Subforums",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Subforums",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AlterColumn<decimal>(
                name: "FinalScore",
                table: "Comments",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(5,4)",
                oldPrecision: 5,
                oldScale: 4,
                oldDefaultValue: 0.5m);
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CodeBattleArena.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddIdPlayerByTaskProgramming : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "TasksProgramming",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "IdPlayer",
                table: "TasksProgramming",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PlayerId",
                table: "TasksProgramming",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TasksProgramming_PlayerId",
                table: "TasksProgramming",
                column: "PlayerId");

            migrationBuilder.AddForeignKey(
                name: "FK_TasksProgramming_AspNetUsers_PlayerId",
                table: "TasksProgramming",
                column: "PlayerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TasksProgramming_AspNetUsers_PlayerId",
                table: "TasksProgramming");

            migrationBuilder.DropIndex(
                name: "IX_TasksProgramming_PlayerId",
                table: "TasksProgramming");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "TasksProgramming");

            migrationBuilder.DropColumn(
                name: "IdPlayer",
                table: "TasksProgramming");

            migrationBuilder.DropColumn(
                name: "PlayerId",
                table: "TasksProgramming");
        }
    }
}

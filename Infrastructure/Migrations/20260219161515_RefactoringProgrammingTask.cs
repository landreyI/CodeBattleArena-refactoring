using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CodeBattleArena.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RefactoringProgrammingTask : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProgrammingTasks_ProgrammingLanguages_ProgrammingLangId",
                table: "ProgrammingTasks");

            migrationBuilder.DropIndex(
                name: "IX_ProgrammingTasks_ProgrammingLangId",
                table: "ProgrammingTasks");

            migrationBuilder.DropColumn(
                name: "Preparation",
                table: "ProgrammingTasks");

            migrationBuilder.DropColumn(
                name: "ProgrammingLangId",
                table: "ProgrammingTasks");

            migrationBuilder.DropColumn(
                name: "VerificationCode",
                table: "ProgrammingTasks");

            migrationBuilder.CreateTable(
                name: "TaskLanguages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProgrammingTaskId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProgrammingLangId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Preparation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VerificationCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsGeneratedAI = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskLanguages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskLanguages_ProgrammingLanguages_ProgrammingLangId",
                        column: x => x.ProgrammingLangId,
                        principalTable: "ProgrammingLanguages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TaskLanguages_ProgrammingTasks_ProgrammingTaskId",
                        column: x => x.ProgrammingTaskId,
                        principalTable: "ProgrammingTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TaskLanguages_ProgrammingLangId",
                table: "TaskLanguages",
                column: "ProgrammingLangId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskLanguages_ProgrammingTaskId_ProgrammingLangId",
                table: "TaskLanguages",
                columns: new[] { "ProgrammingTaskId", "ProgrammingLangId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TaskLanguages");

            migrationBuilder.AddColumn<string>(
                name: "Preparation",
                table: "ProgrammingTasks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "ProgrammingLangId",
                table: "ProgrammingTasks",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "VerificationCode",
                table: "ProgrammingTasks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_ProgrammingTasks_ProgrammingLangId",
                table: "ProgrammingTasks",
                column: "ProgrammingLangId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProgrammingTasks_ProgrammingLanguages_ProgrammingLangId",
                table: "ProgrammingTasks",
                column: "ProgrammingLangId",
                principalTable: "ProgrammingLanguages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

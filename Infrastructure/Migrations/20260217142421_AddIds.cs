using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CodeBattleArena.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_QuestRewards",
                table: "QuestRewards");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlayerSessions",
                table: "PlayerSessions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlayerQuests",
                table: "PlayerQuests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlayerItems",
                table: "PlayerItems");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "QuestRewards",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "PlayerSessions",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "PlayerQuests",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "PlayerItems",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_QuestRewards",
                table: "QuestRewards",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlayerSessions",
                table: "PlayerSessions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlayerQuests",
                table: "PlayerQuests",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlayerItems",
                table: "PlayerItems",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_QuestRewards_QuestId_RewardId",
                table: "QuestRewards",
                columns: new[] { "QuestId", "RewardId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlayerSessions_PlayerId_SessionId",
                table: "PlayerSessions",
                columns: new[] { "PlayerId", "SessionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlayerQuests_PlayerId_QuestId",
                table: "PlayerQuests",
                columns: new[] { "PlayerId", "QuestId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlayerItems_PlayerId_ItemId",
                table: "PlayerItems",
                columns: new[] { "PlayerId", "ItemId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_QuestRewards",
                table: "QuestRewards");

            migrationBuilder.DropIndex(
                name: "IX_QuestRewards_QuestId_RewardId",
                table: "QuestRewards");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlayerSessions",
                table: "PlayerSessions");

            migrationBuilder.DropIndex(
                name: "IX_PlayerSessions_PlayerId_SessionId",
                table: "PlayerSessions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlayerQuests",
                table: "PlayerQuests");

            migrationBuilder.DropIndex(
                name: "IX_PlayerQuests_PlayerId_QuestId",
                table: "PlayerQuests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlayerItems",
                table: "PlayerItems");

            migrationBuilder.DropIndex(
                name: "IX_PlayerItems_PlayerId_ItemId",
                table: "PlayerItems");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "QuestRewards");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "PlayerSessions");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "PlayerQuests");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "PlayerItems");

            migrationBuilder.AddPrimaryKey(
                name: "PK_QuestRewards",
                table: "QuestRewards",
                columns: new[] { "QuestId", "RewardId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlayerSessions",
                table: "PlayerSessions",
                columns: new[] { "PlayerId", "SessionId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlayerQuests",
                table: "PlayerQuests",
                columns: new[] { "PlayerId", "QuestId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlayerItems",
                table: "PlayerItems",
                columns: new[] { "PlayerId", "ItemId" });
        }
    }
}

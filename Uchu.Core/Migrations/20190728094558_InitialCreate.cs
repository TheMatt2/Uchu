﻿using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Uchu.Core.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Username = table.Column<string>(maxLength: 33, nullable: false),
                    Password = table.Column<string>(maxLength: 60, nullable: false),
                    CharacterIndex = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "Characters",
                columns: table => new
                {
                    CharacterId = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Name = table.Column<string>(maxLength: 33, nullable: false),
                    CustomName = table.Column<string>(maxLength: 33, nullable: false),
                    NameRejected = table.Column<bool>(nullable: false),
                    FreeToPlay = table.Column<bool>(nullable: false),
                    ShirtColor = table.Column<long>(nullable: false),
                    ShirtStyle = table.Column<long>(nullable: false),
                    PantsColor = table.Column<long>(nullable: false),
                    HairStyle = table.Column<long>(nullable: false),
                    HairColor = table.Column<long>(nullable: false),
                    Lh = table.Column<long>(nullable: false),
                    Rh = table.Column<long>(nullable: false),
                    EyebrowStyle = table.Column<long>(nullable: false),
                    EyeStyle = table.Column<long>(nullable: false),
                    MouthStyle = table.Column<long>(nullable: false),
                    LastZone = table.Column<int>(nullable: false),
                    LastInstance = table.Column<int>(nullable: false),
                    LastClone = table.Column<long>(nullable: false),
                    LastActivity = table.Column<long>(nullable: false),
                    Level = table.Column<long>(nullable: false),
                    UniverseScore = table.Column<long>(nullable: false),
                    Currency = table.Column<long>(nullable: false),
                    MaximumHealth = table.Column<int>(nullable: false),
                    CurrentHealth = table.Column<int>(nullable: false),
                    MaximumArmor = table.Column<int>(nullable: false),
                    CurrentArmor = table.Column<int>(nullable: false),
                    MaximumImagination = table.Column<int>(nullable: false),
                    CurrentImagination = table.Column<int>(nullable: false),
                    TotalCurrencyCollected = table.Column<long>(nullable: false),
                    TotalBricksCollected = table.Column<long>(nullable: false),
                    TotalSmashablesSmashed = table.Column<long>(nullable: false),
                    TotalQuickBuildsCompleted = table.Column<long>(nullable: false),
                    TotalEnemiesSmashed = table.Column<long>(nullable: false),
                    TotalRocketsUsed = table.Column<long>(nullable: false),
                    TotalMissionsCompleted = table.Column<long>(nullable: false),
                    TotalPetsTamed = table.Column<long>(nullable: false),
                    TotalImaginationPowerUpsCollected = table.Column<long>(nullable: false),
                    TotalLifePowerUpsCollected = table.Column<long>(nullable: false),
                    TotalArmorPowerUpsCollected = table.Column<long>(nullable: false),
                    TotalDistanceTraveled = table.Column<long>(nullable: false),
                    TotalSuicides = table.Column<long>(nullable: false),
                    TotalDamageTaken = table.Column<long>(nullable: false),
                    TotalDamageHealed = table.Column<long>(nullable: false),
                    TotalArmorRepaired = table.Column<long>(nullable: false),
                    TotalImaginationRestored = table.Column<long>(nullable: false),
                    TotalImaginationUsed = table.Column<long>(nullable: false),
                    TotalDistanceDriven = table.Column<long>(nullable: false),
                    TotalTimeAirborne = table.Column<long>(nullable: false),
                    TotalRacingImaginationPowerUpsCollected = table.Column<long>(nullable: false),
                    TotalRacingImaginationCratesSmashed = table.Column<long>(nullable: false),
                    TotalRacecarBoostsActivated = table.Column<long>(nullable: false),
                    TotalRacecarWrecks = table.Column<long>(nullable: false),
                    TotalRacingSmashablesSmashed = table.Column<long>(nullable: false),
                    TotalRacesFinished = table.Column<long>(nullable: false),
                    TotalFirstPlaceFinishes = table.Column<long>(nullable: false),
                    LandingByRocket = table.Column<bool>(nullable: false),
                    Rocket = table.Column<string>(maxLength: 30, nullable: true),
                    UserId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Characters", x => x.CharacterId);
                    table.ForeignKey(
                        name: "FK_Characters_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InventoryItems",
                columns: table => new
                {
                    InventoryItemId = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    LOT = table.Column<int>(nullable: false),
                    Slot = table.Column<int>(nullable: false),
                    Count = table.Column<long>(nullable: false),
                    IsBound = table.Column<bool>(nullable: false),
                    IsEquipped = table.Column<bool>(nullable: false),
                    InventoryType = table.Column<int>(nullable: false),
                    ExtraInfo = table.Column<string>(nullable: true),
                    CharacterId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryItems", x => x.InventoryItemId);
                    table.ForeignKey(
                        name: "FK_InventoryItems_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "CharacterId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Missions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    MissionId = table.Column<int>(nullable: false),
                    State = table.Column<int>(nullable: false),
                    CompletionCount = table.Column<int>(nullable: false),
                    LastCompletion = table.Column<long>(nullable: false),
                    CharacterId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Missions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Missions_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "CharacterId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MissionTasks",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    TaskId = table.Column<int>(nullable: false),
                    Values = table.Column<List<float>>(nullable: false),
                    MissionId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MissionTasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MissionTasks_Missions_MissionId",
                        column: x => x.MissionId,
                        principalTable: "Missions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Characters_UserId",
                table: "Characters",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryItems_CharacterId",
                table: "InventoryItems",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_Missions_CharacterId",
                table: "Missions",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_MissionTasks_MissionId",
                table: "MissionTasks",
                column: "MissionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InventoryItems");

            migrationBuilder.DropTable(
                name: "MissionTasks");

            migrationBuilder.DropTable(
                name: "Missions");

            migrationBuilder.DropTable(
                name: "Characters");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}

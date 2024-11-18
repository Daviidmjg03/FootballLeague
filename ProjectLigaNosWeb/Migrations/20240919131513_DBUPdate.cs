using Microsoft.EntityFrameworkCore.Migrations;

namespace ProjectLigaNosWeb.Migrations
{
    public partial class DBUPdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GameId",
                table: "Statistics",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Statistics_GameId",
                table: "Statistics",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_Statistics_PlayerId",
                table: "Statistics",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_Players_ClubId",
                table: "Players",
                column: "ClubId");

            migrationBuilder.CreateIndex(
                name: "IX_Games_ClubAwayId",
                table: "Games",
                column: "ClubAwayId");

            migrationBuilder.CreateIndex(
                name: "IX_Games_ClubHomeId",
                table: "Games",
                column: "ClubHomeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Games_Clubs_ClubAwayId",
                table: "Games",
                column: "ClubAwayId",
                principalTable: "Clubs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Games_Clubs_ClubHomeId",
                table: "Games",
                column: "ClubHomeId",
                principalTable: "Clubs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Players_Clubs_ClubId",
                table: "Players",
                column: "ClubId",
                principalTable: "Clubs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Statistics_Games_GameId",
                table: "Statistics",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Statistics_Players_PlayerId",
                table: "Statistics",
                column: "PlayerId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Games_Clubs_ClubAwayId",
                table: "Games");

            migrationBuilder.DropForeignKey(
                name: "FK_Games_Clubs_ClubHomeId",
                table: "Games");

            migrationBuilder.DropForeignKey(
                name: "FK_Players_Clubs_ClubId",
                table: "Players");

            migrationBuilder.DropForeignKey(
                name: "FK_Statistics_Games_GameId",
                table: "Statistics");

            migrationBuilder.DropForeignKey(
                name: "FK_Statistics_Players_PlayerId",
                table: "Statistics");

            migrationBuilder.DropIndex(
                name: "IX_Statistics_GameId",
                table: "Statistics");

            migrationBuilder.DropIndex(
                name: "IX_Statistics_PlayerId",
                table: "Statistics");

            migrationBuilder.DropIndex(
                name: "IX_Players_ClubId",
                table: "Players");

            migrationBuilder.DropIndex(
                name: "IX_Games_ClubAwayId",
                table: "Games");

            migrationBuilder.DropIndex(
                name: "IX_Games_ClubHomeId",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "GameId",
                table: "Statistics");
        }
    }
}

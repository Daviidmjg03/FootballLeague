using Microsoft.EntityFrameworkCore.Migrations;

namespace ProjectLigaNosWeb.Migrations
{
    public partial class UpdateDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Players_Clubs_ClubId",
                table: "Players");

            migrationBuilder.DropForeignKey(
                name: "FK_Statistics_Players_PlayerId",
                table: "Statistics");

            migrationBuilder.DropIndex(
                name: "IX_Statistics_PlayerId",
                table: "Statistics");

            migrationBuilder.DropColumn(
                name: "Assists",
                table: "Statistics");

            migrationBuilder.DropColumn(
                name: "PlayerId",
                table: "Statistics");

            migrationBuilder.DropColumn(
                name: "RedCards",
                table: "Statistics");

            migrationBuilder.RenameColumn(
                name: "YellowCards",
                table: "Statistics",
                newName: "ClubId");

            migrationBuilder.AddColumn<int>(
                name: "PlayersId",
                table: "Statistics",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Statistics_ClubId",
                table: "Statistics",
                column: "ClubId");

            migrationBuilder.CreateIndex(
                name: "IX_Statistics_PlayersId",
                table: "Statistics",
                column: "PlayersId");

            migrationBuilder.AddForeignKey(
                name: "FK_Players_Clubs_ClubId",
                table: "Players",
                column: "ClubId",
                principalTable: "Clubs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Statistics_Clubs_ClubId",
                table: "Statistics",
                column: "ClubId",
                principalTable: "Clubs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Statistics_Players_PlayersId",
                table: "Statistics",
                column: "PlayersId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Players_Clubs_ClubId",
                table: "Players");

            migrationBuilder.DropForeignKey(
                name: "FK_Statistics_Clubs_ClubId",
                table: "Statistics");

            migrationBuilder.DropForeignKey(
                name: "FK_Statistics_Players_PlayersId",
                table: "Statistics");

            migrationBuilder.DropIndex(
                name: "IX_Statistics_ClubId",
                table: "Statistics");

            migrationBuilder.DropIndex(
                name: "IX_Statistics_PlayersId",
                table: "Statistics");

            migrationBuilder.DropColumn(
                name: "PlayersId",
                table: "Statistics");

            migrationBuilder.RenameColumn(
                name: "ClubId",
                table: "Statistics",
                newName: "YellowCards");

            migrationBuilder.AddColumn<int>(
                name: "Assists",
                table: "Statistics",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PlayerId",
                table: "Statistics",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RedCards",
                table: "Statistics",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Statistics_PlayerId",
                table: "Statistics",
                column: "PlayerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Players_Clubs_ClubId",
                table: "Players",
                column: "ClubId",
                principalTable: "Clubs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Statistics_Players_PlayerId",
                table: "Statistics",
                column: "PlayerId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

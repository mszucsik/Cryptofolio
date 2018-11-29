using Microsoft.EntityFrameworkCore.Migrations;

namespace Cryptofolio.Data.Migrations
{
    public partial class assetupdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentHigh",
                table: "Asset");

            migrationBuilder.DropColumn(
                name: "CurrentLow",
                table: "Asset");

            migrationBuilder.DropColumn(
                name: "CurrentPrice",
                table: "Asset");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "CurrentHigh",
                table: "Asset",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "CurrentLow",
                table: "Asset",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "CurrentPrice",
                table: "Asset",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}

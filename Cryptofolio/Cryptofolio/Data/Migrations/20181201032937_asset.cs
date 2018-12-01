using Microsoft.EntityFrameworkCore.Migrations;

namespace Cryptofolio.Data.Migrations
{
    public partial class asset : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Current_Price",
                table: "Asset",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Daily_Change",
                table: "Asset",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Current_Price",
                table: "Asset");

            migrationBuilder.DropColumn(
                name: "Daily_Change",
                table: "Asset");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

namespace Cryptofolio.Data.Migrations
{
    public partial class newportfolio : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Daily_Change",
                table: "Portfolio",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Total_Change",
                table: "Portfolio",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Total_Purchased",
                table: "Portfolio",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Daily_Change",
                table: "Portfolio");

            migrationBuilder.DropColumn(
                name: "Total_Change",
                table: "Portfolio");

            migrationBuilder.DropColumn(
                name: "Total_Purchased",
                table: "Portfolio");
        }
    }
}

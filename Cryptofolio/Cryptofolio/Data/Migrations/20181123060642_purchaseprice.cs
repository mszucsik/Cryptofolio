using Microsoft.EntityFrameworkCore.Migrations;

namespace Cryptofolio.Data.Migrations
{
    public partial class purchaseprice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "PurchasePrice",
                table: "Holding",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PurchasePrice",
                table: "Holding");
        }
    }
}

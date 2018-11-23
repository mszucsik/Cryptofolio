using Microsoft.EntityFrameworkCore.Migrations;

namespace Cryptofolio.Data.Migrations
{
    public partial class purchasepricefix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "AssetType",
                table: "Holding",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "AssetType",
                table: "Holding",
                nullable: true,
                oldClrType: typeof(string));
        }
    }
}

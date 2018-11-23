using Microsoft.EntityFrameworkCore.Migrations;

namespace Cryptofolio.Data.Migrations
{
    public partial class assettype : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Holding_Asset_AssetTypeID",
                table: "Holding");

            migrationBuilder.DropIndex(
                name: "IX_Holding_AssetTypeID",
                table: "Holding");

            migrationBuilder.DropColumn(
                name: "AssetTypeID",
                table: "Holding");

            migrationBuilder.AddColumn<string>(
                name: "AssetType",
                table: "Holding",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssetType",
                table: "Holding");

            migrationBuilder.AddColumn<int>(
                name: "AssetTypeID",
                table: "Holding",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Holding_AssetTypeID",
                table: "Holding",
                column: "AssetTypeID");

            migrationBuilder.AddForeignKey(
                name: "FK_Holding_Asset_AssetTypeID",
                table: "Holding",
                column: "AssetTypeID",
                principalTable: "Asset",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

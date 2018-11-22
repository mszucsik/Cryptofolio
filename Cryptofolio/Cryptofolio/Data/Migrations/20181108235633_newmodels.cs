using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Cryptofolio.Data.Migrations
{
    public partial class newmodels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "OwnerID",
                table: "Portfolio",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.CreateTable(
                name: "Asset",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Code = table.Column<int>(nullable: false),
                    CurrentPrice = table.Column<double>(nullable: false),
                    CurrentHigh = table.Column<double>(nullable: false),
                    CurrentLow = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Asset", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Holding",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    OwnerID = table.Column<string>(nullable: true),
                    Creation_Date = table.Column<DateTime>(nullable: false),
                    AssetTypeID = table.Column<int>(nullable: true),
                    Amount = table.Column<double>(nullable: false),
                    PortfolioID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Holding", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Holding_Asset_AssetTypeID",
                        column: x => x.AssetTypeID,
                        principalTable: "Asset",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Holding_Portfolio_PortfolioID",
                        column: x => x.PortfolioID,
                        principalTable: "Portfolio",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Holding_AssetTypeID",
                table: "Holding",
                column: "AssetTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_Holding_PortfolioID",
                table: "Holding",
                column: "PortfolioID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Holding");

            migrationBuilder.DropTable(
                name: "Asset");

            migrationBuilder.AlterColumn<string>(
                name: "OwnerID",
                table: "Portfolio",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}

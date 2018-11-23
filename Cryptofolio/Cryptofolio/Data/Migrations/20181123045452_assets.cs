using Microsoft.EntityFrameworkCore.Migrations;

namespace Cryptofolio.Data.Migrations
{
    public partial class assets : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Asset",
                nullable: true,
                oldClrType: typeof(int));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Code",
                table: "Asset",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}

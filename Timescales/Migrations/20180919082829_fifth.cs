using Microsoft.EntityFrameworkCore.Migrations;

namespace Timescales.Migrations
{
    public partial class fifth : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Timescales",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 50);

            migrationBuilder.AddColumn<string>(
                name: "Placeholder",
                table: "Timescales",
                maxLength: 60,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Timescales_Placeholder",
                table: "Timescales",
                column: "Placeholder",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Timescales_Placeholder",
                table: "Timescales");

            migrationBuilder.DropColumn(
                name: "Placeholder",
                table: "Timescales");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Timescales",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 256);
        }
    }
}

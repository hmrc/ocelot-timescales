using Microsoft.EntityFrameworkCore.Migrations;

namespace Timescales.Migrations
{
    public partial class _9 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Audits_TimescaleId",
                table: "Audits",
                column: "TimescaleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Audits_Timescales_TimescaleId",
                table: "Audits",
                column: "TimescaleId",
                principalTable: "Timescales",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Audits_Timescales_TimescaleId",
                table: "Audits");

            migrationBuilder.DropIndex(
                name: "IX_Audits_TimescaleId",
                table: "Audits");
        }
    }
}

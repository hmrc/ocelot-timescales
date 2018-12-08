using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Timescales.Migrations
{
    public partial class inital : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Timescales",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Placeholder = table.Column<string>(maxLength: 60, nullable: false),
                    Name = table.Column<string>(maxLength: 256, nullable: false),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    Description = table.Column<string>(maxLength: 256, nullable: false),
                    Owners = table.Column<string>(nullable: false),
                    OldestWorkDate = table.Column<DateTime>(nullable: false),
                    Days = table.Column<int>(nullable: false),
                    Basis = table.Column<string>(maxLength: 10, nullable: false),
                    LineOfBusiness = table.Column<string>(maxLength: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Timescales", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Audits",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    TimescaleId = table.Column<Guid>(nullable: false),
                    User = table.Column<string>(unicode: false, fixedLength: true, maxLength: 7, nullable: false),
                    Action = table.Column<string>(maxLength: 7, nullable: false),
                    DateTime = table.Column<DateTime>(nullable: false),
                    OldestWorkDate = table.Column<DateTime>(nullable: false),
                    Days = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Audits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Audits_Timescales_TimescaleId",
                        column: x => x.TimescaleId,
                        principalTable: "Timescales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Audits_TimescaleId",
                table: "Audits",
                column: "TimescaleId");

            migrationBuilder.CreateIndex(
                name: "IX_Timescales_Placeholder",
                table: "Timescales",
                column: "Placeholder",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Audits");

            migrationBuilder.DropTable(
                name: "Timescales");
        }
    }
}

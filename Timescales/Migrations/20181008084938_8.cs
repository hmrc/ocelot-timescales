using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Timescales.Migrations
{
    public partial class _8 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Audits",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    TimescaleId = table.Column<Guid>(nullable: false),
                    User = table.Column<string>(maxLength: 7, nullable: false),
                    Action = table.Column<string>(maxLength: 7, nullable: false),
                    DateTime = table.Column<DateTime>(nullable: false),
                    OldestWorkDate = table.Column<DateTime>(nullable: false),
                    Days = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Audits", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Audits");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HutchManager.Migrations
{
    public partial class DataSources : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TargetDataSourceName",
                table: "ActivitySources",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "DataSources",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    LastCheckin = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataSources", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DataSources");

            migrationBuilder.DropColumn(
                name: "TargetDataSourceName",
                table: "ActivitySources");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LinkLiteManager.Migrations
{
    public partial class UpdateActivitySource : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "ActivitySources",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "ActivitySources");
        }
    }
}

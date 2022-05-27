using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LinkLiteManager.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "ActivitySources");

            migrationBuilder.AddColumn<string>(
                name: "TypeId",
                table: "ActivitySources",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SourceTypes",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SourceTypes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActivitySources_TypeId",
                table: "ActivitySources",
                column: "TypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_ActivitySources_SourceTypes_TypeId",
                table: "ActivitySources",
                column: "TypeId",
                principalTable: "SourceTypes",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActivitySources_SourceTypes_TypeId",
                table: "ActivitySources");

            migrationBuilder.DropTable(
                name: "SourceTypes");

            migrationBuilder.DropIndex(
                name: "IX_ActivitySources_TypeId",
                table: "ActivitySources");

            migrationBuilder.DropColumn(
                name: "TypeId",
                table: "ActivitySources");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "ActivitySources",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}

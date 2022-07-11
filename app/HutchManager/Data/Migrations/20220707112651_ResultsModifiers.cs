using System.Text.Json;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace HutchManager.Migrations
{
    public partial class ResultsModifiers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ActivitySourceModel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Host = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    ResourceId = table.Column<string>(type: "text", nullable: false),
                    DisplayName = table.Column<string>(type: "text", nullable: false),
                    TargetDataSourceName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivitySourceModel", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ModifierTypeModel",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Limit = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModifierTypeModel", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ResultsModifier",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    ActivitySourceId = table.Column<int>(type: "integer", nullable: false),
                    TypeId = table.Column<string>(type: "text", nullable: true),
                    Parameters = table.Column<JsonDocument>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResultsModifier", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResultsModifier_ActivitySourceModel_ActivitySourceId",
                        column: x => x.ActivitySourceId,
                        principalTable: "ActivitySourceModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ResultsModifier_ModifierTypeModel_TypeId",
                        column: x => x.TypeId,
                        principalTable: "ModifierTypeModel",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ResultsModifier_ActivitySourceId",
                table: "ResultsModifier",
                column: "ActivitySourceId");

            migrationBuilder.CreateIndex(
                name: "IX_ResultsModifier_TypeId",
                table: "ResultsModifier",
                column: "TypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ResultsModifier");

            migrationBuilder.DropTable(
                name: "ActivitySourceModel");

            migrationBuilder.DropTable(
                name: "ModifierTypeModel");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace HutchManager.Migrations
{
    public partial class ModifierType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ResultsModifier_ActivitySourceModel_ActivitySourceId",
                table: "ResultsModifier");

            migrationBuilder.DropForeignKey(
                name: "FK_ResultsModifier_ModifierTypeModel_TypeId",
                table: "ResultsModifier");

            migrationBuilder.DropTable(
                name: "ActivitySourceModel");

            migrationBuilder.DropTable(
                name: "ModifierTypeModel");

            migrationBuilder.AlterColumn<string>(
                name: "TypeId",
                table: "ResultsModifier",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "ModifierTypes",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Limit = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModifierTypes", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_ResultsModifier_ActivitySources_ActivitySourceId",
                table: "ResultsModifier",
                column: "ActivitySourceId",
                principalTable: "ActivitySources",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ResultsModifier_ModifierTypes_TypeId",
                table: "ResultsModifier",
                column: "TypeId",
                principalTable: "ModifierTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ResultsModifier_ActivitySources_ActivitySourceId",
                table: "ResultsModifier");

            migrationBuilder.DropForeignKey(
                name: "FK_ResultsModifier_ModifierTypes_TypeId",
                table: "ResultsModifier");

            migrationBuilder.DropTable(
                name: "ModifierTypes");

            migrationBuilder.AlterColumn<string>(
                name: "TypeId",
                table: "ResultsModifier",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateTable(
                name: "ActivitySourceModel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DisplayName = table.Column<string>(type: "text", nullable: false),
                    Host = table.Column<string>(type: "text", nullable: false),
                    ResourceId = table.Column<string>(type: "text", nullable: false),
                    TargetDataSourceName = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false)
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

            migrationBuilder.AddForeignKey(
                name: "FK_ResultsModifier_ActivitySourceModel_ActivitySourceId",
                table: "ResultsModifier",
                column: "ActivitySourceId",
                principalTable: "ActivitySourceModel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ResultsModifier_ModifierTypeModel_TypeId",
                table: "ResultsModifier",
                column: "TypeId",
                principalTable: "ModifierTypeModel",
                principalColumn: "Id");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace HutchManager.Migrations
{
    public partial class AgentEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ResultsModifier_ActivitySources_ActivitySourceId",
                table: "ResultsModifier");

            migrationBuilder.DropForeignKey(
                name: "FK_ResultsModifier_ModifierTypes_TypeId",
                table: "ResultsModifier");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ResultsModifier",
                table: "ResultsModifier");

            migrationBuilder.RenameTable(
                name: "ResultsModifier",
                newName: "ResultsModifiers");

            migrationBuilder.RenameIndex(
                name: "IX_ResultsModifier_TypeId",
                table: "ResultsModifiers",
                newName: "IX_ResultsModifiers_TypeId");

            migrationBuilder.RenameIndex(
                name: "IX_ResultsModifier_ActivitySourceId",
                table: "ResultsModifiers",
                newName: "IX_ResultsModifiers_ActivitySourceId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ResultsModifiers",
                table: "ResultsModifiers",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Agents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    ClientId = table.Column<string>(type: "text", nullable: false),
                    ClientSecretHash = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Agents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AgentDataSource",
                columns: table => new
                {
                    AgentsId = table.Column<int>(type: "integer", nullable: false),
                    DataSourcesId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgentDataSource", x => new { x.AgentsId, x.DataSourcesId });
                    table.ForeignKey(
                        name: "FK_AgentDataSource_Agents_AgentsId",
                        column: x => x.AgentsId,
                        principalTable: "Agents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AgentDataSource_DataSources_DataSourcesId",
                        column: x => x.DataSourcesId,
                        principalTable: "DataSources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AgentDataSource_DataSourcesId",
                table: "AgentDataSource",
                column: "DataSourcesId");

            migrationBuilder.AddForeignKey(
                name: "FK_ResultsModifiers_ActivitySources_ActivitySourceId",
                table: "ResultsModifiers",
                column: "ActivitySourceId",
                principalTable: "ActivitySources",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ResultsModifiers_ModifierTypes_TypeId",
                table: "ResultsModifiers",
                column: "TypeId",
                principalTable: "ModifierTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ResultsModifiers_ActivitySources_ActivitySourceId",
                table: "ResultsModifiers");

            migrationBuilder.DropForeignKey(
                name: "FK_ResultsModifiers_ModifierTypes_TypeId",
                table: "ResultsModifiers");

            migrationBuilder.DropTable(
                name: "AgentDataSource");

            migrationBuilder.DropTable(
                name: "Agents");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ResultsModifiers",
                table: "ResultsModifiers");

            migrationBuilder.RenameTable(
                name: "ResultsModifiers",
                newName: "ResultsModifier");

            migrationBuilder.RenameIndex(
                name: "IX_ResultsModifiers_TypeId",
                table: "ResultsModifier",
                newName: "IX_ResultsModifier_TypeId");

            migrationBuilder.RenameIndex(
                name: "IX_ResultsModifiers_ActivitySourceId",
                table: "ResultsModifier",
                newName: "IX_ResultsModifier_ActivitySourceId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ResultsModifier",
                table: "ResultsModifier",
                column: "Id");

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
    }
}

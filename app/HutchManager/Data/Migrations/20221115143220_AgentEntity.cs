using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace HutchManager.Migrations
{
    public partial class AgentEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AgentId",
                table: "DataSources",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Agent",
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
                    table.PrimaryKey("PK_Agent", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DataSources_AgentId",
                table: "DataSources",
                column: "AgentId");

            migrationBuilder.AddForeignKey(
                name: "FK_DataSources_Agent_AgentId",
                table: "DataSources",
                column: "AgentId",
                principalTable: "Agent",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DataSources_Agent_AgentId",
                table: "DataSources");

            migrationBuilder.DropTable(
                name: "Agent");

            migrationBuilder.DropIndex(
                name: "IX_DataSources_AgentId",
                table: "DataSources");

            migrationBuilder.DropColumn(
                name: "AgentId",
                table: "DataSources");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HutchManager.Migrations
{
    public partial class _20220715_rename_logs_to_Logs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_logs",
                table: "logs");

            migrationBuilder.RenameTable(
                name: "logs",
                newName: "Logs");

            migrationBuilder.RenameColumn(
                name: "messagetemplate",
                table: "Logs",
                newName: "message_template");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Logs",
                table: "Logs",
                column: "id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Logs",
                table: "Logs");

            migrationBuilder.RenameTable(
                name: "Logs",
                newName: "logs");

            migrationBuilder.RenameColumn(
                name: "message_template",
                table: "logs",
                newName: "messagetemplate");

            migrationBuilder.AddPrimaryKey(
                name: "PK_logs",
                table: "logs",
                column: "id");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HutchManager.Migrations
{
    public partial class RemoveUnderScoreFromMessageTemplate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "message_template",
                table: "Logs",
                newName: "messagetemplate");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "messagetemplate",
                table: "Logs",
                newName: "message_template");
        }
    }
}

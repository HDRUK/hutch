using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HutchAgent.Migrations
{
    /// <inheritdoc />
    public partial class AddExitCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ExitCode",
                table: "WfexsJobs",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExitCode",
                table: "WfexsJobs");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HutchAgent.Migrations
{
    /// <inheritdoc />
    public partial class AddResultsPathAndExitCode : Migration
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

            migrationBuilder.AddColumn<string>(
                name: "ResultsPath",
                table: "WfexsJobs",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExitCode",
                table: "WfexsJobs");

            migrationBuilder.DropColumn(
                name: "ResultsPath",
                table: "WfexsJobs");
        }
    }
}

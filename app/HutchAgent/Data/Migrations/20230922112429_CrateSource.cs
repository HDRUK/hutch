using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HutchAgent.Migrations
{
    /// <inheritdoc />
    public partial class CrateSource : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OutputUrl",
                table: "WorkflowJobs");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "WorkflowJobs");

            migrationBuilder.DropColumn(
                name: "ProjectName",
                table: "WorkflowJobs");

            migrationBuilder.RenameColumn(
                name: "OutputAccess",
                table: "WorkflowJobs",
                newName: "EgressTarget");

            migrationBuilder.AddColumn<string>(
                name: "CrateSource",
                table: "WorkflowJobs",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CrateSource",
                table: "WorkflowJobs");

            migrationBuilder.RenameColumn(
                name: "EgressTarget",
                table: "WorkflowJobs",
                newName: "OutputAccess");

            migrationBuilder.AddColumn<string>(
                name: "OutputUrl",
                table: "WorkflowJobs",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ProjectId",
                table: "WorkflowJobs",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ProjectName",
                table: "WorkflowJobs",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}

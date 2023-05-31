using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HutchAgent.Migrations
{
    /// <inheritdoc />
    public partial class AddWfexsJobPid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Pid",
                table: "WfexsJobs",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Pid",
                table: "WfexsJobs");
        }
    }
}

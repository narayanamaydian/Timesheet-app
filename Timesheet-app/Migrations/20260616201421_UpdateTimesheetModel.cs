using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Timesheet_app.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTimesheetModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "Timesheets");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProjectId",
                table: "Timesheets",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}

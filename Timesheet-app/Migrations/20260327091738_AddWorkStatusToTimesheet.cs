using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Timesheet_app.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkStatusToTimesheet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "WFO",
                table: "Timesheets",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WFO",
                table: "Timesheets");
        }
    }
}

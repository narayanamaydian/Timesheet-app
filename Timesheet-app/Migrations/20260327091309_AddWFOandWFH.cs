using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Timesheet_app.Migrations
{
    /// <inheritdoc />
    public partial class AddWFOandWFH : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Timesheets_Users_UserId",
                table: "Timesheets");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Timesheets",
                newName: "UserID");

            migrationBuilder.RenameIndex(
                name: "IX_Timesheets_UserId",
                table: "Timesheets",
                newName: "IX_Timesheets_UserID");

            migrationBuilder.AlterColumn<string>(
                name: "UserID",
                table: "Timesheets",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Timesheets_Users_UserID",
                table: "Timesheets",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Timesheets_Users_UserID",
                table: "Timesheets");

            migrationBuilder.RenameColumn(
                name: "UserID",
                table: "Timesheets",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Timesheets_UserID",
                table: "Timesheets",
                newName: "IX_Timesheets_UserId");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Timesheets",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddForeignKey(
                name: "FK_Timesheets_Users_UserId",
                table: "Timesheets",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}

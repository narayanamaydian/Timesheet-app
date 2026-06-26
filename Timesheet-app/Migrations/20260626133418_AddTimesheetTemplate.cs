using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Timesheet_app.Migrations
{
    /// <inheritdoc />
    public partial class AddTimesheetTemplate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CompanyId",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CompanyModelId",
                table: "Users",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Companies",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TimesheetTemplates",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BaseTemplateId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsBased = table.Column<bool>(type: "bit", nullable: false),
                    CompanyModelId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimesheetTemplates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TimesheetTemplates_Companies_CompanyModelId",
                        column: x => x.CompanyModelId,
                        principalTable: "Companies",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TimesheetTemplateHeaders",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Header = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TemplateId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TimesheetTemplateModelId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimesheetTemplateHeaders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TimesheetTemplateHeaders_TimesheetTemplates_TimesheetTemplateModelId",
                        column: x => x.TimesheetTemplateModelId,
                        principalTable: "TimesheetTemplates",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_CompanyModelId",
                table: "Users",
                column: "CompanyModelId");

            migrationBuilder.CreateIndex(
                name: "IX_TimesheetTemplateHeaders_TimesheetTemplateModelId",
                table: "TimesheetTemplateHeaders",
                column: "TimesheetTemplateModelId");

            migrationBuilder.CreateIndex(
                name: "IX_TimesheetTemplates_CompanyModelId",
                table: "TimesheetTemplates",
                column: "CompanyModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Companies_CompanyModelId",
                table: "Users",
                column: "CompanyModelId",
                principalTable: "Companies",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Companies_CompanyModelId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "TimesheetTemplateHeaders");

            migrationBuilder.DropTable(
                name: "TimesheetTemplates");

            migrationBuilder.DropTable(
                name: "Companies");

            migrationBuilder.DropIndex(
                name: "IX_Users_CompanyModelId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CompanyModelId",
                table: "Users");
        }
    }
}

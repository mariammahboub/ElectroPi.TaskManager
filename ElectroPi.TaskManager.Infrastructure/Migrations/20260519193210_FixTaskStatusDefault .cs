using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElectroPi.TaskManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixTaskStatusDefault : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "ProjectTasks",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldDefaultValue: "Todo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "ProjectTasks",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "Todo",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MessageApi.Migrations
{
    /// <inheritdoc />
    public partial class AddColumnsCheck : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Check",
                table: "Aplication",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Check",
                table: "Aplication");
        }
    }
}

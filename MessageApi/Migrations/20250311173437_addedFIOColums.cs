using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MessageApi.Migrations
{
    /// <inheritdoc />
    public partial class addedFIOColums : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FIO",
                table: "Aplication",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FIO",
                table: "Aplication");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebSystemOne.Migrations
{
    /// <inheritdoc />
    public partial class _2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Aplication_AspNetUsers_UserId",
                table: "Aplication");

            migrationBuilder.DropForeignKey(
                name: "FK_Aplication_Service_ServiceId",
                table: "Aplication");

            migrationBuilder.DropForeignKey(
                name: "FK_Aplication_Status_StatusId",
                table: "Aplication");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserModelId",
                table: "Aplication",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ServiceModelId",
                table: "Aplication",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StatusModelId",
                table: "Aplication",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Aplication_ApplicationUserModelId",
                table: "Aplication",
                column: "ApplicationUserModelId");

            migrationBuilder.CreateIndex(
                name: "IX_Aplication_ServiceModelId",
                table: "Aplication",
                column: "ServiceModelId");

            migrationBuilder.CreateIndex(
                name: "IX_Aplication_StatusModelId",
                table: "Aplication",
                column: "StatusModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Aplication_AspNetUsers_ApplicationUserModelId",
                table: "Aplication",
                column: "ApplicationUserModelId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Aplication_AspNetUsers_UserId",
                table: "Aplication",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Aplication_Service_ServiceId",
                table: "Aplication",
                column: "ServiceId",
                principalTable: "Service",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Aplication_Service_ServiceModelId",
                table: "Aplication",
                column: "ServiceModelId",
                principalTable: "Service",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Aplication_Status_StatusId",
                table: "Aplication",
                column: "StatusId",
                principalTable: "Status",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Aplication_Status_StatusModelId",
                table: "Aplication",
                column: "StatusModelId",
                principalTable: "Status",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Aplication_AspNetUsers_ApplicationUserModelId",
                table: "Aplication");

            migrationBuilder.DropForeignKey(
                name: "FK_Aplication_AspNetUsers_UserId",
                table: "Aplication");

            migrationBuilder.DropForeignKey(
                name: "FK_Aplication_Service_ServiceId",
                table: "Aplication");

            migrationBuilder.DropForeignKey(
                name: "FK_Aplication_Service_ServiceModelId",
                table: "Aplication");

            migrationBuilder.DropForeignKey(
                name: "FK_Aplication_Status_StatusId",
                table: "Aplication");

            migrationBuilder.DropForeignKey(
                name: "FK_Aplication_Status_StatusModelId",
                table: "Aplication");

            migrationBuilder.DropIndex(
                name: "IX_Aplication_ApplicationUserModelId",
                table: "Aplication");

            migrationBuilder.DropIndex(
                name: "IX_Aplication_ServiceModelId",
                table: "Aplication");

            migrationBuilder.DropIndex(
                name: "IX_Aplication_StatusModelId",
                table: "Aplication");

            migrationBuilder.DropColumn(
                name: "ApplicationUserModelId",
                table: "Aplication");

            migrationBuilder.DropColumn(
                name: "ServiceModelId",
                table: "Aplication");

            migrationBuilder.DropColumn(
                name: "StatusModelId",
                table: "Aplication");

            migrationBuilder.AddForeignKey(
                name: "FK_Aplication_AspNetUsers_UserId",
                table: "Aplication",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Aplication_Service_ServiceId",
                table: "Aplication",
                column: "ServiceId",
                principalTable: "Service",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Aplication_Status_StatusId",
                table: "Aplication",
                column: "StatusId",
                principalTable: "Status",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

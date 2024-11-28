using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApplicationTracker.Migrations
{
    /// <inheritdoc />
    public partial class ApplicationSchemaChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Applications_WorkEnvironments_EnvironmentId",
                table: "Applications");

            migrationBuilder.RenameColumn(
                name: "EnvironmentId",
                table: "Applications",
                newName: "WorkEnvironmentId");

            migrationBuilder.RenameIndex(
                name: "IX_Applications_EnvironmentId",
                table: "Applications",
                newName: "IX_Applications_WorkEnvironmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Applications_WorkEnvironments_WorkEnvironmentId",
                table: "Applications",
                column: "WorkEnvironmentId",
                principalTable: "WorkEnvironments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Applications_WorkEnvironments_WorkEnvironmentId",
                table: "Applications");

            migrationBuilder.RenameColumn(
                name: "WorkEnvironmentId",
                table: "Applications",
                newName: "EnvironmentId");

            migrationBuilder.RenameIndex(
                name: "IX_Applications_WorkEnvironmentId",
                table: "Applications",
                newName: "IX_Applications_EnvironmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Applications_WorkEnvironments_EnvironmentId",
                table: "Applications",
                column: "EnvironmentId",
                principalTable: "WorkEnvironments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

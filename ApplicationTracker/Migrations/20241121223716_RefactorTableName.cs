using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApplicationTracker.Migrations
{
    /// <inheritdoc />
    public partial class RefactorTableName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Applications_Environments_EnvironmentId",
                table: "Applications");

            migrationBuilder.DropForeignKey(
                name: "FK_Applications_Environments_Id",
                table: "Applications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Environments",
                table: "Environments");

            migrationBuilder.RenameTable(
                name: "Environments",
                newName: "WorkEnvironments");

            migrationBuilder.RenameIndex(
                name: "IX_Environments_Name",
                table: "WorkEnvironments",
                newName: "IX_WorkEnvironments_Name");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WorkEnvironments",
                table: "WorkEnvironments",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Applications_WorkEnvironments_EnvironmentId",
                table: "Applications",
                column: "EnvironmentId",
                principalTable: "WorkEnvironments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Applications_WorkEnvironments_Id",
                table: "Applications",
                column: "Id",
                principalTable: "WorkEnvironments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Applications_WorkEnvironments_EnvironmentId",
                table: "Applications");

            migrationBuilder.DropForeignKey(
                name: "FK_Applications_WorkEnvironments_Id",
                table: "Applications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WorkEnvironments",
                table: "WorkEnvironments");

            migrationBuilder.RenameTable(
                name: "WorkEnvironments",
                newName: "Environments");

            migrationBuilder.RenameIndex(
                name: "IX_WorkEnvironments_Name",
                table: "Environments",
                newName: "IX_Environments_Name");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Environments",
                table: "Environments",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Applications_Environments_EnvironmentId",
                table: "Applications",
                column: "EnvironmentId",
                principalTable: "Environments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Applications_Environments_Id",
                table: "Applications",
                column: "Id",
                principalTable: "Environments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

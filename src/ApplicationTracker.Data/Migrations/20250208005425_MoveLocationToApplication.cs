using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApplicationTracker.Data.Migrations
{
    /// <inheritdoc />
    public partial class MoveLocationToApplication : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Applications_Locations_LocationId",
                table: "Applications");

            migrationBuilder.DropTable(
                name: "Locations");

            migrationBuilder.DropIndex(
                name: "IX_Applications_LocationId",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "Applications");

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Applications",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "State",
                table: "Applications",
                type: "nvarchar(2)",
                maxLength: 2,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "City",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "State",
                table: "Applications");

            migrationBuilder.AddColumn<int>(
                name: "LocationId",
                table: "Applications",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Country = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    State = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Applications_LocationId",
                table: "Applications",
                column: "LocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Applications_Locations_LocationId",
                table: "Applications",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

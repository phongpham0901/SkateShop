using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkateShop.Migrations
{
    /// <inheritdoc />
    public partial class ElevenMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Weight",
                table: "Products",
                newName: "Type");

            migrationBuilder.RenameColumn(
                name: "Hollow",
                table: "Products",
                newName: "Size");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Type",
                table: "Products",
                newName: "Weight");

            migrationBuilder.RenameColumn(
                name: "Size",
                table: "Products",
                newName: "Hollow");
        }
    }
}

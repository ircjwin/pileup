using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Piles.Migrations
{
    /// <inheritdoc />
    public partial class AddIsSilenced : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSilenced",
                table: "Ruminations",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSilenced",
                table: "Ruminations");
        }
    }
}

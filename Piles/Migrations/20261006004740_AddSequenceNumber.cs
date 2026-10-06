using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Piles.Migrations
{
    /// <inheritdoc />
    public partial class AddSequenceNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SequenceNumber",
                table: "Ruminations",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql("UPDATE Ruminations SET SequenceNumber = Origin");

            migrationBuilder.AddColumn<int>(
                name: "SequenceNumber",
                table: "Piles",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql("UPDATE Piles SET SequenceNumber = Origin");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SequenceNumber",
                table: "Ruminations");

            migrationBuilder.DropColumn(
                name: "SequenceNumber",
                table: "Piles");
        }
    }
}

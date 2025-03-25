using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PensionSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addedIsValidated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsValidated",
                table: "Contributions",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsValidated",
                table: "Contributions");
        }
    }
}

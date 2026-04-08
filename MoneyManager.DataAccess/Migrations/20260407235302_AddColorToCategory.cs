using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MoneyManager.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddColorToCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Color",
                table: "Category",
                type: "int",
                nullable: false,
                defaultValue: 2309453);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Color",
                table: "Category");
        }
    }
}

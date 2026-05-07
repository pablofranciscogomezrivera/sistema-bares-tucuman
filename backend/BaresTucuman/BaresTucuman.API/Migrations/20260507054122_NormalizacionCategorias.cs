using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BaresTucuman.API.Migrations
{
    /// <inheritdoc />
    public partial class NormalizacionCategorias : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CategoriaAMostrar",
                table: "Bares",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CategoriaAMostrar",
                table: "Bares");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Proyecto_Restaurante.Migrations
{
    /// <inheritdoc />
    public partial class AgregarEstadoMesa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Estado",
                table: "Mesa",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                defaultValue: "Disponible");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Estado",
                table: "Mesa");
        }
    }
}

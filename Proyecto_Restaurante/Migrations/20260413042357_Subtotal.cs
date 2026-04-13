using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Proyecto_Restaurante.Migrations
{
    public partial class Subtotal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Subtotal",
                table: "Factura",
                type: "decimal(18,2)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Subtotal",
                table: "Factura");
        }
    }
}
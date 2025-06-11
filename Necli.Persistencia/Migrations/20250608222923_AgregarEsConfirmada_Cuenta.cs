using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Necli.Persistencia.Migrations
{
    /// <inheritdoc />
    public partial class AgregarEsConfirmada_Cuenta : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "EsConfirmada",
                table: "Cuenta",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EsConfirmada",
                table: "Cuenta");
        }
    }
}

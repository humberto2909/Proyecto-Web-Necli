using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Necli.Persistencia.Migrations
{
    /// <inheritdoc />
    public partial class AgregarTokenConfirmacion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TokenConfirmacion",
                table: "Cuenta",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TokenConfirmacion",
                table: "Cuenta");
        }
    }
}

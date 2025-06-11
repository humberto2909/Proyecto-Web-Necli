using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Necli.Persistencia.Migrations
{
    /// <inheritdoc />
    public partial class AgregarCamposTokenReset : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FechaTokenReset",
                table: "Usuario",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TokenResetPassword",
                table: "Usuario",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FechaTokenReset",
                table: "Usuario");

            migrationBuilder.DropColumn(
                name: "TokenResetPassword",
                table: "Usuario");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Necli.Persistencia.Migrations
{
    /// <inheritdoc />
    public partial class ActualizarUsuarioModelo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Cuenta_UsuarioId",
                table: "Cuenta");

            migrationBuilder.DropColumn(
                name: "CuentaConfirmada",
                table: "Usuario");

            migrationBuilder.DropColumn(
                name: "EmailPendiente",
                table: "Usuario");

            migrationBuilder.DropColumn(
                name: "NuevaContrasenaPendiente",
                table: "Usuario");

            migrationBuilder.DropColumn(
                name: "TokenCambioContrasena",
                table: "Usuario");

            migrationBuilder.DropColumn(
                name: "TokenCambioCorreo",
                table: "Usuario");

            migrationBuilder.DropColumn(
                name: "TokenConfirmacion",
                table: "Usuario");

            migrationBuilder.DropColumn(
                name: "TokenExpiracion",
                table: "Usuario");

            migrationBuilder.DropColumn(
                name: "TokenRestablecimiento",
                table: "Usuario");

            migrationBuilder.CreateIndex(
                name: "IX_Cuenta_UsuarioId",
                table: "Cuenta",
                column: "UsuarioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Cuenta_UsuarioId",
                table: "Cuenta");

            migrationBuilder.AddColumn<bool>(
                name: "CuentaConfirmada",
                table: "Usuario",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmailPendiente",
                table: "Usuario",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NuevaContrasenaPendiente",
                table: "Usuario",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TokenCambioContrasena",
                table: "Usuario",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TokenCambioCorreo",
                table: "Usuario",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TokenConfirmacion",
                table: "Usuario",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "TokenExpiracion",
                table: "Usuario",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TokenRestablecimiento",
                table: "Usuario",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cuenta_UsuarioId",
                table: "Cuenta",
                column: "UsuarioId",
                unique: true);
        }
    }
}

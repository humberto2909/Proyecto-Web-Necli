using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Necli.Logica.Dtos;
using Necli.Logica.Interface;
using Necli.Logica.Service;

namespace ProyectoNecli.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CuentaController : ControllerBase
    {
        private readonly ICuentaService _cuentaService;
        private readonly ICorreoService _correoService;
        private readonly IConfiguration _config;


        public CuentaController(ICuentaService cuentaService, ICorreoService correoService, IConfiguration config)
        {
            _cuentaService = cuentaService;
            _correoService = correoService;
            _config = config;
        }

        [HttpPost("Crear-Cuenta")]
        public IActionResult CrearCuenta([FromBody] CrearCuentaDto dto)
        {
            try
            {
                _cuentaService.CrearCuenta(dto);
                return Ok("Cuenta creada correctamente.");
            }
            catch (Exception ex) 
            {
                return BadRequest($"Datos inválidos: {ex.Message}");
            }
        }

        [HttpPost("confirmar-token-de-cuenta")]
        public IActionResult ConfirmarCuentaPorToken([FromBody] string token)
        {
            try
            {
                var cuenta = _cuentaService.ObtenerPorToken(token);
                if (cuenta == null)
                    return NotFound("❌ Token inválido o no asociado a ninguna cuenta.");

                if (cuenta.EsConfirmada)
                    return BadRequest("⚠️ Esta cuenta ya fue confirmada.");

                cuenta.EsConfirmada = true;
                cuenta.TokenConfirmacion = null;

                _cuentaService.Actualizar(cuenta);

                return Ok("✅ Tu cuenta ha sido confirmada exitosamente.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"❌ Error al confirmar cuenta: {ex.Message}");
            }
        }


        [HttpGet("buscar-cuenta-por-telefono/{telefono}")]
        public IActionResult ObtenerCuentaPorTelefono(string telefono)
        {
            var cuenta = _cuentaService.ObtenerPorTelefono(telefono);
            if (cuenta == null)
                return NotFound("Cuenta no encontrada.");

            return Ok(new
            {
                cuenta.Telefono,
                cuenta.NombreTitular,
                cuenta.Saldo
            });
        }



        [HttpDelete("eliminar-cuenta-por-telefono/{telefono}")]
        [Authorize]
        public IActionResult EliminarCuentaPorTelefono(string telefono)
        {
            var idUsuarioDesdeToken = int.Parse(User.FindFirst("IdUsuario")?.Value ?? "0");

            var cuenta = _cuentaService.ObtenerPorTelefono(telefono);
            if (cuenta == null)
                return NotFound("Cuenta no encontrada.");

            if (cuenta.UsuarioId != idUsuarioDesdeToken)
                return StatusCode(403, "No tienes permiso para eliminar esta cuenta.");

            if (!cuenta.EsConfirmada)
                return BadRequest("No puedes eliminar una cuenta que no ha sido confirmada.");

            if (cuenta.Saldo >= 50000)
                return BadRequest("Solo puedes eliminar cuentas con saldo menor a $50.000.");

            var eliminado = _cuentaService.EliminarCuenta(cuenta.IdCuenta);

            return eliminado ? Ok("Cuenta eliminada correctamente.") : StatusCode(500, "Error al eliminar la cuenta.");
        }


        [HttpPost("reenviar-confirmacion")]
        public IActionResult ReenviarConfirmacion([FromQuery] string telefono)
        {
            var cuenta = _cuentaService.ObtenerPorTelefono(telefono);
            if (cuenta == null)
                return NotFound("❌ No se encontró ninguna cuenta con ese número.");

            if (cuenta.EsConfirmada)
                return BadRequest("⚠️ Esta cuenta ya está confirmada.");

            if (string.IsNullOrWhiteSpace(telefono))
                return BadRequest("Número de teléfono no proporcionado.");

            // Generar nuevo token
            var nuevoToken = Guid.NewGuid().ToString();
            cuenta.TokenConfirmacion = nuevoToken;

            _cuentaService.Actualizar(cuenta);

            // Construir link
            string html = $@"
            <h2>🔁 Reenvío de código de confirmación</h2>
            <p>Hola {cuenta.Usuario.NombreUsuario},</p>
            <p>Este es tu nuevo código de confirmación:</p>
            <p><strong>{nuevoToken}</strong></p>
            <p>Cópialo y pégalo en la aplicación para confirmar tu cuenta.</p>";

            _correoService.EnviarCorreo(
                cuenta.Usuario.Email,
                "🔐 Reenvío de confirmación de cuenta NECLI",
                html,
                null
            );

            return Ok("📨 Se ha reenviado el correo de confirmación.");
        }

    }
}
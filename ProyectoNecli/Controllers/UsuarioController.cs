using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Necli.Logica.Dtos;
using Necli.Logica.Interface;
using Necli.Logica.Service;
using System.Security.Claims;

namespace ProyectoNecli.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;
        private readonly ITransaccionService _transaccionService;
        private readonly ICuentaService _cuentaService;

        public UsuarioController(IUsuarioService usuarioService, ITransaccionService transaccionService, ICuentaService cuentaService)
        {
            _usuarioService = usuarioService;
            _transaccionService = transaccionService;
            _cuentaService = cuentaService;
        }

        [HttpGet("Obtener-Usuario-Por-Cedula")]
        public IActionResult ObtenerUsuarioPorIdentificacion([FromQuery] string cedula)
        {
            var usuario = _usuarioService.ObtenerUsuarioPorIdentificacion(cedula);
            if (usuario == null)
                return NotFound("Usuario no encontrado.");

            return Ok(new
            {
                usuario.Cedula,
                usuario.NombreUsuario,
                usuario.ApellidoUsuario,
                usuario.Email,
                usuario.FechaNacimiento,
                usuario.TipoUsuario
            });
        }

        [Authorize]
        [HttpPut("Actualizar-Usuario")]
        public IActionResult ActualizarUsuario([FromQuery] string cedula, [FromBody] ActualizarUsuarioDto dto)
        {
            try
            {
                var idUsuarioDesdeToken = int.Parse(User.FindFirst("IdUsuario")?.Value ?? "0");

                var cuenta = _cuentaService.ObtenerCuentaPorUsuario(idUsuarioDesdeToken);
                if (cuenta == null || cuenta.Usuario.Cedula != cedula)
                    return StatusCode(403, "No tienes permiso para modificar esta información.");

                var usuarioActualizado = _usuarioService.ActualizarUsuario(cedula, dto);

                return Ok(new
                {
                    usuarioActualizado.Cedula,
                    usuarioActualizado.NombreUsuario,
                    usuarioActualizado.ApellidoUsuario,
                    usuarioActualizado.Email
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpDelete("Eliminar-Usuario")]
        [Authorize]
        public IActionResult EliminarUsuario([FromQuery] string cedula)
        {
            try
            {
                if (_usuarioService.EliminarUsuario(cedula))
                    return Ok("✅ Usuario eliminado correctamente.");
                else
                    return NotFound("❌ No se encontró el usuario.");
            }
            catch (Exception ex)
            {
                return BadRequest($"⚠️ {ex.Message}");
            }
        }

        [Authorize]
        [HttpPost("solicitar-restablecimiento")]
        public IActionResult SolicitarReset([FromQuery] string correo)
        {
            try
            {
                var idUsuarioDesdeToken = int.Parse(User.FindFirst("IdUsuario")?.Value ?? "0");
                var cuenta = _cuentaService.ObtenerCuentaPorUsuario(idUsuarioDesdeToken);

                if (cuenta == null || cuenta.Usuario.Email != correo)
                    return StatusCode(403, "No tienes permiso para restablecer la contraseña de este correo.");

                _usuarioService.GenerarTokenReset(correo);
                return Ok("📩 Se ha enviado el token de restablecimiento.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPost("restablecer-contraseña")]
        public IActionResult Restablecer([FromQuery] string token, [FromBody] string nuevaClave)
        {
            var ok = _usuarioService.RestablecerContrasena(token, nuevaClave);
            return ok ? Ok("✅ Contraseña actualizada.") : BadRequest("❌ Token inválido o vencido.");
        }

    }
}

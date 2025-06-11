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
    public class TransaccionController : ControllerBase
    {
        private readonly ICuentaService _cuentaService;
        private readonly ITransaccionService _transaccionService;
        private readonly ICorreoService _correoService;


        public TransaccionController(
            ICuentaService cuentaService,
            ITransaccionService transaccionService,
            ICorreoService correoService)
        {
            _cuentaService = cuentaService;
            _transaccionService = transaccionService;
            _correoService = correoService;
        }


        [Authorize]
        [HttpPost("Realizar-transaccion")]
        public IActionResult CrearTransaccion([FromBody] CrearTransaccionDto dto)
        {
            var claim = User.FindFirst("IdUsuario");
            if (claim == null)
                return Unauthorized("Token inválido o sin 'IdUsuario'.");

            int idUsuario = int.Parse(claim.Value);

            var cuentaOrigen = _cuentaService.ObtenerCuentaPorUsuario(idUsuario);
            if (cuentaOrigen == null)
                return BadRequest("No se encontró una cuenta asociada al usuario.");

            try
            {
                var transaccion = _transaccionService.CrearTransaccion(cuentaOrigen.Telefono, dto);
                return Created("", transaccion);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [Authorize]
        [HttpGet("buscar-transaccion-por-filtros")]
        public IActionResult ObtenerPorFiltro([FromQuery] FiltroTransaccionDto filtro)
        {
            var idCuentaOrigen = int.Parse(User.FindFirst("IdCuenta")?.Value ?? "0");
            var transacciones = _transaccionService.ObtenerPorFiltro(idCuentaOrigen, filtro);
            return Ok(transacciones);
        }



        [HttpPost("generar-mi-reporte")]
        [Authorize]
        public IActionResult GenerarMiReporte()
        {
            try
            {
                var idUsuario = ObtenerIdUsuarioDesdeToken();

                var cuenta = _cuentaService.ObtenerCuentaPorUsuario(idUsuario);
                if (cuenta == null)
                    return NotFound("Cuenta no encontrada.");

                if (string.IsNullOrWhiteSpace(cuenta.Usuario?.Cedula))
                    return BadRequest("❌ No se puede generar el reporte: la cédula del usuario es inválida.");

                var transacciones = _transaccionService.ObtenerTransaccionesDelMesAnterior()
                                                       .FirstOrDefault(kvp => kvp.Key.IdCuenta == cuenta.IdCuenta).Value;

                if (transacciones == null || !transacciones.Any())
                    return BadRequest("No hay transacciones del mes anterior.");

                var reporteService = new ReporteMensualService();
                var rutaPdf = reporteService.GenerarPdfProtegido(cuenta, transacciones);

                if (string.IsNullOrEmpty(rutaPdf))
                    return StatusCode(500, "❌ No se pudo generar el PDF correctamente.");

                _correoService.EnviarCorreo(
                    cuenta.Usuario.Email,
                    "📊 Tu reporte mensual de transacciones",
                    $"<p>Hola {cuenta.Usuario.NombreUsuario},<br>Adjunto encontrarás tu reporte del mes {DateTime.UtcNow.Month:D2}/{DateTime.UtcNow.Year}.</p>",
                    rutaPdf
                );

                return Ok("📧 Reporte generado y enviado a tu correo.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"❌ Error al generar tu reporte: {ex.Message}");
            }
        }


        private int ObtenerIdUsuarioDesdeToken()
        {
            var idClaim = User.Claims.FirstOrDefault(c => c.Type == "IdUsuario");
            return idClaim != null ? int.Parse(idClaim.Value) : 0;
        }


        [HttpPost("generar-reportes-mensuales")]
        public IActionResult GenerarReportesMensuales()
        {
            try
            {
                _transaccionService.GenerarReportesMensuales();
                return Ok("✅ Todos los PDFs fueron generados correctamente.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"❌ Error al generar reportes: {ex.Message}");
            }
        }

    }
}

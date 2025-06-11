using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Necli.Logica.Dtos;
using Necli.Logica.Interface;
using System.Text.Json;

namespace ProyectoNecli.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransaccionExternaController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;
        private readonly ITransaccionService _transaccionService;

        public TransaccionExternaController(IHttpClientFactory httpClientFactory, IConfiguration config, ITransaccionService transaccionService)
        {
            _httpClientFactory = httpClientFactory;
            _config = config;
            _transaccionService = transaccionService;
        }

        [HttpPost("realizar-transaccion-externa")]
        [Authorize]
        public async Task<IActionResult> RealizarTransaccionExterna([FromBody] CrearTransaccionExternaDto dto)
        {
            // Validar límites de monto
            int minimo = int.Parse(_config["LimitesTransacciones:Minimo"]);
            int maximo = int.Parse(_config["LimitesTransacciones:Maximo"]);
            if (dto.Monto < minimo || dto.Monto > maximo)
                throw new Exception($"El monto debe estar entre ${minimo:N0} y ${maximo:N0} COP.");

            // Construir URL con parámetros
            string baseUrl = _config["InterbankApi:ValidationEndpoint"];
            string url = $"{baseUrl}?numeroCuenta={dto.NumeroCuenta}&documentoUsuario={dto.NumeroDocumento}&banco={dto.BancoId}";

            var client = _httpClientFactory.CreateClient();

            try
            {
                var response = await client.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                    return NotFound("❌ Cuenta externa no válida.");

                // Si es Bancolombia (1558), reflejar de inmediato
                if (dto.BancoId == "1558") // Bancolombia
                {
                    var idCuentaOrigen = int.Parse(User.FindFirst("IdCuenta")?.Value ?? "0");

                    _transaccionService.RegistrarTransaccionExterna(idCuentaOrigen, dto);

                    return Ok(new
                    {
                        estado = "reflejado",
                        banco = "Bancolombia",
                        fecha = DateTime.Now,
                        monto = dto.Monto,
                        cuenta = dto.NumeroCuenta
                    });
                }


                // Otros bancos: reflejo en 2 horas
                return Ok(new
                {
                    estado = "pendiente",
                    banco = dto.BancoId,
                    fechaReflejo = DateTime.Now.AddHours(2),
                    monto = dto.Monto,
                    cuenta = dto.NumeroCuenta
                });
            }
            catch
            {
                return StatusCode(500, "Error al conectar con el servicio bancario externo.");
            }
        }

        [HttpGet("consultar-transaccion-externa")]
        [Authorize]
        public async Task<IActionResult> ConsultarTransaccionesExternas([FromQuery] string numeroCuenta, [FromQuery] DateTime fecha)
        {
            string baseUrl = _config["InterbankApi:TransactionEndpoint"];
            string url = $"{baseUrl}?numeroCuenta={numeroCuenta}&fecha={fecha:yyyy-MM-dd}";

            var client = _httpClientFactory.CreateClient();

            try
            {
                var response = await client.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                    return NotFound("No se encontraron transacciones externas.");

                var contenido = await response.Content.ReadAsStringAsync();
                return Ok(JsonSerializer.Deserialize<object>(contenido));
            }
            catch
            {
                return StatusCode(500, "Error al consultar las transacciones externas.");
            }
        }
    }
}
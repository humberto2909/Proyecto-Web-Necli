using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Necli.Logica.Dtos;
using Necli.Logica.Interface;
using Necli.Persistencia.Interface;
using Necli.Persistencia.Utils;

namespace ProyectoNecli.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ICuentaRepository _cuentaRepository;
        private readonly IJwtService _jwtService;

        public LoginController(IUsuarioRepository usuarioRepo, ICuentaRepository cuentaRepo, IJwtService jwtService)
        {
            _usuarioRepository = usuarioRepo;
            _cuentaRepository = cuentaRepo;
            _jwtService = jwtService;
        }

        [HttpPost]
        public IActionResult Login([FromBody] LoginDto dto)
        {
            var usuario = _usuarioRepository.ObtenerPorEmail(dto.Email);
            if (usuario == null || usuario.Contrasena != SeguridadUtil.HashearContrasena(dto.Contrasena))
                return Unauthorized("Credenciales inválidas.");

            var cuenta = _cuentaRepository.ObtenerPorTelefono(dto.Telefono);
            if (cuenta == null)
                return NotFound("Cuenta no encontrada.");

            var token = _jwtService.GenerarToken(usuario.IdUsuario, cuenta.IdCuenta, usuario.TipoUsuario);
            return Ok(new { token });
        }
    }
}

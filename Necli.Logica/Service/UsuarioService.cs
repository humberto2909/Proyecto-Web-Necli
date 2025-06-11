using Microsoft.Extensions.Configuration;
using Necli.Entidades.Entidades;
using Necli.Logica.Dtos;
using Necli.Logica.Interface;
using Necli.Persistencia.Interface;
using Necli.Persistencia.Repository;
using Necli.Persistencia.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace Necli.Logica.Service
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ICorreoService _correoService;
        private readonly ICuentaRepository _cuentaRepository;
        private readonly IConfiguration _config;


        public UsuarioService(IUsuarioRepository usuarioRepository, ICuentaRepository cuentaRepository, ICorreoService correoService, IConfiguration config)
        {
            _usuarioRepository = usuarioRepository;
            _correoService = correoService;
            _cuentaRepository = cuentaRepository;
            _config = config;
        }

        public Usuario CrearUsuario(CrearUsuarioDto dto)
        {
            int edad = DateTime.Today.Year - dto.FechaNacimiento.Year;
            if (dto.FechaNacimiento.Date > DateTime.Today.AddYears(-edad)) edad--;

            if (edad < 15)
                throw new Exception("El usuario debe tener al menos 15 años.");

            if (_usuarioRepository.EmailExistente(dto.Email))
                throw new Exception("El correo ya está registrado.");

            if (_usuarioRepository.CedulaExistente(dto.Cedula))
                throw new Exception("La cédula ya está registrada.");

            if (!Regex.IsMatch(dto.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                throw new Exception("El formato del correo no es válido.");

            var usuario = new Usuario
            {
                TipoUsuario = dto.TipoUsuario,
                Contrasena = SeguridadUtil.HashearContrasena(dto.Contrasena),
                Cedula = dto.Cedula,
                NombreUsuario = dto.NombreUsuario,
                ApellidoUsuario = dto.ApellidoUsuario,
                Email = dto.Email,
                FechaNacimiento = dto.FechaNacimiento
            };

            return _usuarioRepository.InsertarUsuario(usuario);
        }

        public Usuario ObtenerUsuarioPorIdentificacion(string cedula)
        {
            return _usuarioRepository.ObtenerPorCedula(cedula);
        }

        public Usuario ActualizarUsuario(string cedula, ActualizarUsuarioDto dto)
        {
            var usuario = _usuarioRepository.ObtenerPorCedula(cedula);
            if (usuario == null)
                throw new Exception("Usuario no encontrado.");

            // Validar que el nuevo correo no esté en uso por otro usuario
            var existente = _usuarioRepository.ObtenerPorEmail(dto.Email);
            if (existente != null && existente.IdUsuario != usuario.IdUsuario)
                throw new Exception("Ya existe otro usuario con este correo.");

            // Actualizar solo los campos permitidos
            usuario.NombreUsuario = dto.NombreUsuario;
            usuario.ApellidoUsuario = dto.ApellidoUsuario;
            usuario.Email = dto.Email;

            _usuarioRepository.Actualizar(usuario);

            string html = $@"
                <h2>🔄 Cambios en tu cuenta NECLI</h2>
                <p>Se han actualizado tus datos personales.</p>";

            _correoService.EnviarCorreo(
                usuario.Email,
                "Actualización de datos en NECLI",
                html,
                null
            );

            return usuario;
        }




        public bool VerificarCredenciales(string email, string contrasena)
        {
            var usuario = _usuarioRepository.ObtenerPorEmail(email);
            return usuario != null && usuario.Contrasena == contrasena;
        }

        public bool EliminarUsuario(string cedula)
        {
            var usuario = _usuarioRepository.ObtenerPorCedula(cedula);
            if (usuario == null)
                return false;

            var cuentas = _cuentaRepository.ObtenerCuentasPorUsuario(usuario.IdUsuario);
            if (cuentas.Any(c => c.Saldo > 0))
                throw new Exception("No se puede eliminar el usuario: tiene cuentas con saldo mayor a 0.");

            return _usuarioRepository.EliminarUsuario(usuario);
        }

        public void GenerarTokenReset(string email)
        {
            var usuario = _usuarioRepository.ObtenerPorEmail(email);
            if (usuario == null)
                throw new Exception("No existe un usuario con ese correo.");

            usuario.TokenResetPassword = Guid.NewGuid().ToString();
            usuario.FechaTokenReset = DateTime.Now;

            _usuarioRepository.Actualizar(usuario);

            string baseUrl = _config["Frontend:BaseUrl"] ?? "http://localhost:5005";
            string html = $@"
                <h2>🔐 Solicitud de restablecimiento de contraseña</h2>
                <p>Este es tu token de restablecimiento:</p>
                <p><strong>{usuario.TokenResetPassword}</strong></p>
                <p>Cópialo y pégalo en la plataforma para establecer una nueva contraseña.</p>";


            _correoService.EnviarCorreo(
                usuario.Email,
                "Restablecer contraseña - NECLI",
                html,
                null
            );
        }

        public bool RestablecerContrasena(string token, string nuevaClave)
        {
            var usuario = _usuarioRepository.ObtenerPorTokenReset(token);
            if (usuario == null || usuario.FechaTokenReset == null || (DateTime.Now - usuario.FechaTokenReset.Value).TotalMinutes > 30)
                return false; // Token inválido o vencido

            usuario.Contrasena = SeguridadUtil.HashearContrasena(nuevaClave);
            usuario.TokenResetPassword = null;
            usuario.FechaTokenReset = null;

            _usuarioRepository.Actualizar(usuario);

            string html = $@"
                <h2>✅ Contraseña actualizada</h2>
                <p>Tu contraseña ha sido cambiada correctamente.</p>";

            _correoService.EnviarCorreo(
                usuario.Email,
                "Contraseña actualizada",
                html,
                null
            );

            return true;
        }


    }

}

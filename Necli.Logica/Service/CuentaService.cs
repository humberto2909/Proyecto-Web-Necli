using Necli.Entidades.Entidades;
using Necli.Logica.Dtos;
using Necli.Logica.Interface;
using Necli.Persistencia.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Necli.Logica.Service
{
    public class CuentaService : ICuentaService
    {
        private readonly ICuentaRepository _cuentaRepository;
        private readonly IUsuarioService _usuarioService;
        private readonly ICorreoService _correoService;

        public CuentaService(ICuentaRepository cuentaRepository, IUsuarioService usuarioService, ICorreoService correoService)
        {
            _cuentaRepository = cuentaRepository;
            _usuarioService = usuarioService;
            _correoService = correoService;
        }

        public Cuenta CrearCuenta(CrearCuentaDto dto)
        {
            // Validar formato del teléfono
            if (!Regex.IsMatch(dto.Telefono, @"^\d{10}$"))
                throw new Exception("❌ El número de teléfono debe tener exactamente 10 dígitos.");

            // Validar que el teléfono no esté en uso
            if (_cuentaRepository.TelefonoExistente(dto.Telefono))

                throw new Exception("❌ El número de teléfono ya está registrado.");

            // Crear usuario asociado
            var nuevoUsuario = _usuarioService.CrearUsuario(dto.Usuario);

            // Generar token de confirmación
            var token = Guid.NewGuid().ToString();

            // Crear cuenta
            var cuenta = new Cuenta
            {
                NombreTitular = dto.NombreTitular,
                FechaCreacion = DateTime.Now,
                Saldo = 0,
                Telefono = dto.Telefono,
                UsuarioId = nuevoUsuario.IdUsuario,
                TokenConfirmacion = token,
                EsConfirmada = false
            };

            // Insertar cuenta en BD
            var cuentaInsertada = _cuentaRepository.InsertarCuenta(cuenta);

            // Enviar correo de confirmación
            string html = $@"
            <h2>Bienvenido a NECLI</h2>
            <p>Gracias por registrarte, {dto.Usuario.NombreUsuario}.</p>
            <p>Este es tu código de confirmación:</p>
            <p><strong>{token}</strong></p>
            <p>Cópialo y pégalo en la app para confirmar tu cuenta.</p>";

            _correoService.EnviarCorreo(dto.Usuario.Email, "🔐 Confirma tu cuenta NECLI", html, null);

            return cuentaInsertada;
        }

        public bool EliminarCuenta(int idCuenta)
        {
            var cuenta = _cuentaRepository.ObtenerPorId(idCuenta);
            if (cuenta == null) return false;

            return _cuentaRepository.EliminarCuenta(idCuenta);
        }

        public Cuenta ObtenerPorId(int idCuenta)
        {
            return _cuentaRepository.ObtenerPorId(idCuenta);
        }

        public Cuenta ObtenerCuentaPorUsuario(int idUsuario)
        {
            return _cuentaRepository.ObtenerCuentaPorUsuario(idUsuario);
        }

        public Cuenta ObtenerPorToken(string token)
        {
            return _cuentaRepository.ObtenerPorToken(token);
        }

        public void Actualizar(Cuenta cuenta)
        {
            _cuentaRepository.Actualizar(cuenta);
        }

        public bool CuentaEstaConfirmada(int cuentaId)
        {
            var cuenta = _cuentaRepository.ObtenerPorId(cuentaId);
            return cuenta != null && cuenta.EsConfirmada;
        }
        public Cuenta ObtenerPorTelefono(string telefono)
        {
            return _cuentaRepository.ObtenerPorTelefono(telefono);
        }

    }
}

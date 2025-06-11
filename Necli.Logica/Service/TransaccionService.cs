using Microsoft.EntityFrameworkCore;
using Necli.Entidades.Entidades;
using Necli.Logica.Dtos;
using Necli.Logica.Interface;
using Necli.Persistencia.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Necli.Logica.Service;


namespace Necli.Logica.Service
{
    public class TransaccionService : ITransaccionService
    {
        private readonly ITransaccionRepository _transaccionRepository;
        private readonly ICuentaRepository _cuentaRepository;
        private readonly ICorreoService _correoService;
        private readonly IConfiguration _config;


        public TransaccionService(
            ITransaccionRepository transaccionRepository, ICuentaRepository cuentaRepository, ICorreoService correoService, IConfiguration config)
        {
            _transaccionRepository = transaccionRepository;
            _cuentaRepository = cuentaRepository;
            _correoService = correoService;
            _config = config;
        }


        public Transaccion CrearTransaccion(string telefonoOrigen, CrearTransaccionDto dto)
        {
            int minimo = int.Parse(_config["LimitesTransacciones:Minimo"]);
            int maximo = int.Parse(_config["LimitesTransacciones:Maximo"]);

            if (dto.Monto < minimo || dto.Monto > maximo)
                throw new Exception($"El monto debe estar entre ${minimo:N0} y ${maximo:N0} COP.");

            if (telefonoOrigen == dto.TelefonoDestino)
                throw new Exception("❌ No puedes transferir a la misma cuenta.");

            var origen = _cuentaRepository.ObtenerPorTelefono(telefonoOrigen);
            if (origen == null)
                throw new Exception("❌ Cuenta de origen no encontrada.");

            var destino = _cuentaRepository.ObtenerPorTelefono(dto.TelefonoDestino);
            if (destino == null)
                throw new Exception("❌ Cuenta de destino no encontrada.");

            if (origen.Saldo < dto.Monto)
                throw new Exception("❌ Saldo insuficiente.");

            origen.Saldo -= dto.Monto;
            destino.Saldo += dto.Monto;

            var transaccion = new Transaccion
            {
                CuentaOrigenId = origen.IdCuenta,
                CuentaDestinoId = destino.IdCuenta,
                Monto = dto.Monto,
                TipoTransaccion = "salida",
                FechaTransaccion = DateTime.Now
            };

            _cuentaRepository.Actualizar(origen);
            _cuentaRepository.Actualizar(destino);
            _transaccionRepository.InsertarTransaccion(transaccion);

            return transaccion;
        }




        public Transaccion ObtenerTransaccionPorId(int id)
        {
            return _transaccionRepository.ObtenerPorId(id);
        }



        public IEnumerable<Transaccion> ListarTransaccionesFiltradas(DateTime? desde, DateTime? hasta, int cuentaOrigenId, int? cuentaDestinoId)
        {
            return _transaccionRepository.ListarFiltradas(desde, hasta, cuentaOrigenId, cuentaDestinoId);
        }




        public IEnumerable<Transaccion> ObtenerPorFiltro(int idCuentaOrigen, FiltroTransaccionDto filtro)
        {
            var query = _transaccionRepository.ObtenerTodo()
                .Where(t => t.CuentaOrigenId == idCuentaOrigen);

            if (filtro.FechaDesde.HasValue)
                query = query.Where(t => t.FechaTransaccion >= filtro.FechaDesde.Value);

            if (filtro.FechaHasta.HasValue)
                query = query.Where(t => t.FechaTransaccion <= filtro.FechaHasta.Value);

            if (filtro.CuentaDestinoId.HasValue)
                query = query.Where(t => t.CuentaDestinoId == filtro.CuentaDestinoId.Value);

            return query.ToList();
        }


        public Dictionary<Cuenta, List<Transaccion>> ObtenerTransaccionesDelMesAnterior()
        {
            var resultado = new Dictionary<Cuenta, List<Transaccion>>();

            var hoy = DateTime.UtcNow;
            var mesAnterior = hoy; // Para pruebas quito el .AddMonths(-1) y uso hoy solamente

            var cuentas = _cuentaRepository.ObtenerTodasConUsuario();

            foreach (var cuenta in cuentas)
            {
                var transacciones = _transaccionRepository
                    .ObtenerTransaccionesDelMes(cuenta.IdCuenta, mesAnterior.Year, mesAnterior.Month)
                    .ToList();

                if (transacciones.Any())
                {
                    resultado.Add(cuenta, transacciones);
                }
            }

            return resultado;
        }



        public void GenerarReportesMensuales()
        {
            Console.WriteLine("📊 Iniciando generación de reportes mensuales...");

            var transaccionesPorCuenta = ObtenerTransaccionesDelMesAnterior();
            if (transaccionesPorCuenta == null || !transaccionesPorCuenta.Any())
            {
                Console.WriteLine("⚠️ No se encontraron transacciones para generar reportes.");
                return;
            }

            var reporteService = new ReporteMensualService();

            foreach (var kvp in transaccionesPorCuenta)
            {
                var cuenta = kvp.Key;
                var transacciones = kvp.Value;

                try
                {
                    var rutaPdf = reporteService.GenerarPdfProtegido(cuenta, transacciones);
                    if (string.IsNullOrEmpty(rutaPdf))
                    {
                        Console.WriteLine("❌ Error: ruta del PDF generada es nula.");
                        continue;
                    }

                    Console.WriteLine($"✅ PDF generado correctamente para {cuenta.Usuario.Cedula}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Error al generar PDF para {cuenta.Usuario.Email}: {ex.Message}");
                }
            }
        }







        public void EnviarReporteTransaccionesPorCorreo(int idUsuario)
        {
            var cuenta = _cuentaRepository.ObtenerCuentaPorUsuario(idUsuario);
            if (cuenta == null)
                throw new Exception("Cuenta no encontrada.");

            var transacciones = ObtenerTransaccionesDelMesAnterior()
                .FirstOrDefault(kvp => kvp.Key.IdCuenta == cuenta.IdCuenta).Value;

            if (transacciones == null || !transacciones.Any())
                throw new Exception("No hay transacciones del mes anterior.");

            var reporteService = new ReporteMensualService();
            var rutaPdf = reporteService.GenerarPdfProtegido(cuenta, transacciones);

            _correoService.EnviarConAdjunto(
                cuenta.Usuario.Email,
                "📄 Informe mensual de transacciones",
                "Adjunto encontrarás tu reporte en PDF del mes anterior.",
                rutaPdf
            );
        }



        public void RegistrarTransaccionExterna(int cuentaOrigenId, CrearTransaccionExternaDto dto)
        {
            var cuentaOrigen = _cuentaRepository.ObtenerPorId(cuentaOrigenId);
            if (cuentaOrigen == null)
                throw new Exception("Cuenta origen no encontrada.");

            if (cuentaOrigen.Saldo < dto.Monto)
                throw new Exception("Saldo insuficiente.");

            // Simulación: Cuenta destino es externa y se crea una cuenta ficticia local 
            var cuentaDestino = new Cuenta
            {
                NombreTitular = $"Banco Externo {dto.BancoId}",
                Telefono = dto.NumeroCuenta,
                FechaCreacion = DateTime.Now,
                Saldo = 0,
                EsConfirmada = true,
                UsuarioId = cuentaOrigen.UsuarioId 
            };

            // Insertar cuenta destino temporal
            _cuentaRepository.InsertarCuenta(cuentaDestino);

            // Registrar la transacción
            var transaccion = new Transaccion
            {
                FechaTransaccion = DateTime.Now,
                Monto = dto.Monto,
                TipoTransaccion = "salida",
                CuentaOrigenId = cuentaOrigen.IdCuenta,
                CuentaDestinoId = cuentaDestino.IdCuenta
            };

            _transaccionRepository.InsertarTransaccion(transaccion);

            // Descontar saldo
            cuentaOrigen.Saldo -= dto.Monto;
            _cuentaRepository.Actualizar(cuentaOrigen);
        }


    }
}

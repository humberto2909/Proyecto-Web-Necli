using Necli.Entidades.Entidades;
using Necli.Logica.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Necli.Logica.Interface
{
    public interface ITransaccionService
    {
        Transaccion CrearTransaccion(string telefonoOrigen, CrearTransaccionDto dto);
        Transaccion ObtenerTransaccionPorId(int id);
        IEnumerable<Transaccion> ListarTransaccionesFiltradas(DateTime? desde, DateTime? hasta, int cuentaOrigenId, int? cuentaDestinoId);
        IEnumerable<Transaccion> ObtenerPorFiltro(int idCuentaOrigen, FiltroTransaccionDto filtro);
        void GenerarReportesMensuales();
        Dictionary<Cuenta, List<Transaccion>> ObtenerTransaccionesDelMesAnterior();
        void EnviarReporteTransaccionesPorCorreo(int idUsuario);
        void RegistrarTransaccionExterna(int cuentaOrigenId, CrearTransaccionExternaDto dto);

    }
}

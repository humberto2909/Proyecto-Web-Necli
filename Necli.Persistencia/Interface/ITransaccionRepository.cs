using Necli.Entidades.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Necli.Persistencia.Interface
{
    public interface ITransaccionRepository
    {
        Transaccion InsertarTransaccion(Transaccion transaccion);
        Transaccion ObtenerPorId(int id);
        IEnumerable<Transaccion> ListarFiltradas(DateTime? desde, DateTime? hasta, int cuentaOrigenId, int? cuentaDestinoId);
        IQueryable<Transaccion> ObtenerTodo();
        IEnumerable<Transaccion> ObtenerTransaccionesDelMes(int cuentaOrigenId, int anio, int mes);

    }
}

using Microsoft.EntityFrameworkCore;
using Necli.Entidades.Entidades;
using Necli.Persistencia.DbContext;
using Necli.Persistencia.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Necli.Persistencia.Repository
{
    public class TransaccionRepository : ITransaccionRepository
    {
        private readonly NecliDbContext _context;

        public TransaccionRepository(NecliDbContext context)
        {
            _context = context;
        }

        public Transaccion InsertarTransaccion(Transaccion transaccion)
        {
            _context.Transacciones.Add(transaccion);
            _context.SaveChanges();
            return transaccion;
        }

        public Transaccion ObtenerPorId(int id)
        {
            return _context.Transacciones.FirstOrDefault(t => t.IdTransaccion == id);
        }

        public IEnumerable<Transaccion> ListarFiltradas(DateTime? desde, DateTime? hasta, int cuentaOrigenId, int? cuentaDestinoId)
        {
            var query = _context.Transacciones.AsQueryable();

            query = query.Where(t => t.CuentaOrigenId == cuentaOrigenId);

            if (desde.HasValue)
                query = query.Where(t => t.FechaTransaccion >= desde.Value);

            if (hasta.HasValue)
                query = query.Where(t => t.FechaTransaccion <= hasta.Value);

            if (cuentaDestinoId.HasValue)
                query = query.Where(t => t.CuentaDestinoId == cuentaDestinoId.Value);

            return query.ToList();
        }

        public IQueryable<Transaccion> ObtenerTodo()
        {
            return _context.Transacciones
                .Include(t => t.CuentaDestino)
                .Include(t => t.CuentaOrigen);
        }

        public IEnumerable<Transaccion> ObtenerTransaccionesDelMes(int cuentaOrigenId, int anio, int mes)
        {
            return _context.Transacciones
                .Where(t => t.CuentaOrigenId == cuentaOrigenId &&
                            t.FechaTransaccion.Year == anio &&
                            t.FechaTransaccion.Month == mes)
                .ToList();
        }


    }
}

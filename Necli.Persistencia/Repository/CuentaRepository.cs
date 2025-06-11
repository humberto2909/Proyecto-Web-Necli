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
    public class CuentaRepository : ICuentaRepository
    {
        private readonly NecliDbContext _context;

        public CuentaRepository(NecliDbContext context)
        {
            _context = context;
        }

        public Cuenta InsertarCuenta(Cuenta cuenta)
        {
            _context.Cuentas.Add(cuenta);
            _context.SaveChanges();
            return cuenta;
        }

        public Cuenta ObtenerPorId(int idCuenta)
        {
            return _context.Cuentas.FirstOrDefault(c => c.IdCuenta == idCuenta);
        }

        public Cuenta ObtenerPorTelefono(string telefono)
        {
            return _context.Cuentas
                   .Include(c => c.Usuario) 
                   .FirstOrDefault(c => c.Telefono == telefono);

        }

        public bool PuedeEliminarCuenta(int idCuenta)
        {
            var cuenta = _context.Cuentas.Find(idCuenta);
            return cuenta != null && cuenta.Saldo < 50000;
        }

        public bool EliminarCuenta(int idCuenta)
        {
            var cuenta = _context.Cuentas.Find(idCuenta);
            if (cuenta == null) return false;

            _context.Cuentas.Remove(cuenta);
            _context.SaveChanges();
            return true;
        }

        public decimal ObtenerSaldo(int idCuenta)
        {
            var cuenta = _context.Cuentas.Find(idCuenta);
            return cuenta?.Saldo ?? 0;
        }
        public IEnumerable<Cuenta> ObtenerTodasConUsuario()
        {
            return _context.Cuentas.Include(c => c.Usuario).ToList();
        }

        public IEnumerable<Cuenta> ObtenerCuentasPorUsuario(int usuarioId)
        {
            return _context.Cuentas.Where(c => c.UsuarioId == usuarioId).ToList();
        }

        public Cuenta ObtenerCuentaPorUsuario(int usuarioId)
        {
            return _context.Cuentas.Include(c => c.Usuario)
                                   .FirstOrDefault(c => c.UsuarioId == usuarioId);
        }

        public bool TelefonoExistente(string telefono)
        {
            return _context.Cuentas.Any(c => c.Telefono == telefono);
        }

        public void Actualizar(Cuenta cuenta)
        {
            _context.Cuentas.Update(cuenta);
            _context.SaveChanges();
        }

        public Cuenta ObtenerPorToken(string token)
        {
            return _context.Cuentas.Include(c => c.Usuario)
                                   .FirstOrDefault(c => c.TokenConfirmacion == token);
        }

    }
}

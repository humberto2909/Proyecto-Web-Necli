using Necli.Entidades.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Necli.Persistencia.Interface
{
    public interface ICuentaRepository
    {
        Cuenta InsertarCuenta(Cuenta cuenta);
        Cuenta ObtenerPorId(int idCuenta);
        Cuenta ObtenerPorTelefono(string telefono);
        bool PuedeEliminarCuenta(int idCuenta);
        bool EliminarCuenta(int idCuenta);
        decimal ObtenerSaldo(int idCuenta);
        IEnumerable<Cuenta> ObtenerTodasConUsuario();
        IEnumerable<Cuenta> ObtenerCuentasPorUsuario(int usuarioId);
        Cuenta ObtenerCuentaPorUsuario(int usuarioId);
        bool TelefonoExistente(string telefono);
        void Actualizar(Cuenta cuenta);
        Cuenta ObtenerPorToken(string token);
    }
}

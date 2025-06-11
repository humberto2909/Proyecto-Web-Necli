using Necli.Entidades.Entidades;
using Necli.Logica.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Necli.Logica.Interface
{
    public interface ICuentaService
    {
        Cuenta CrearCuenta(CrearCuentaDto dto);
        bool EliminarCuenta(int idCuenta);
        Cuenta ObtenerPorId(int idCuenta);
        Cuenta ObtenerCuentaPorUsuario(int idUsuario);
        Cuenta ObtenerPorToken(string token);
        void Actualizar(Cuenta cuenta);
        bool CuentaEstaConfirmada(int cuentaId);
        Cuenta ObtenerPorTelefono(string telefono);
    }
}

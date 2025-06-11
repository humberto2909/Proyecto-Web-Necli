using Necli.Entidades.Entidades;
using Necli.Logica.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Necli.Logica.Interface
{

    public interface IUsuarioService
    {
        Usuario CrearUsuario(CrearUsuarioDto dto);
        Usuario ObtenerUsuarioPorIdentificacion(string cedula);
        Usuario ActualizarUsuario(string cedula, ActualizarUsuarioDto dto);
        bool VerificarCredenciales(string email, string contrasena);
        bool EliminarUsuario(string cedula);
        void GenerarTokenReset(string email);
        bool RestablecerContrasena(string token, string nuevaClave);

    }
}

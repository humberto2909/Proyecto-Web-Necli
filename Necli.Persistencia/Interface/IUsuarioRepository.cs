using Necli.Entidades.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Necli.Persistencia.Interface
{
    public interface IUsuarioRepository
    {
        Usuario InsertarUsuario(Usuario usuario);
        Usuario ObtenerPorCedula(string cedula);
        Usuario ObtenerPorEmail(string email);
        bool EmailExistente(string email);
        bool EliminarUsuario(Usuario usuario);
        Usuario ObtenerMedianteEmail(string email);
        bool CedulaExistente(string cedula);
        Usuario ObtenerPorTokenReset(string token);
        void Actualizar(Usuario usuario);

    }
}

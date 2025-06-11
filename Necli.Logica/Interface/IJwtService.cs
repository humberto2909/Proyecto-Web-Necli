using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Necli.Logica.Interface
{
    public interface IJwtService
    {
        string GenerarToken(int idUsuario, int idCuenta, string tipoUsuario);
    }
}

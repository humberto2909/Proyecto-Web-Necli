using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Necli.Logica.Dtos
{
    public class CrearCuentaDto
    {
        public string NombreTitular { get; set; }
        public string Telefono { get; set; }
        public CrearUsuarioDto Usuario { get; set; }
    }
}

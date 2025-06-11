using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Necli.Logica.Dtos
{
    public class CrearUsuarioDto
    {
        public string TipoUsuario { get; set; }
        public string Contrasena { get; set; }
        public string Cedula { get; set; }
        public string NombreUsuario { get; set; }
        public string ApellidoUsuario { get; set; }
        public string Email { get; set; }
        public DateTime FechaNacimiento { get; set; }
    }
}

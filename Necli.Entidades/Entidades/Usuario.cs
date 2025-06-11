using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Necli.Entidades.Entidades
{
    public class Usuario
    {
        [Key]
        public int IdUsuario { get; set; }
        public required string TipoUsuario { get; set; }
        public required string Contrasena { get; set; }
        public required string Cedula { get; set; }
        public required string NombreUsuario { get; set; }
        public required string ApellidoUsuario { get; set; }
        public required string Email { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public string? TokenResetPassword { get; set; }
        public DateTime? FechaTokenReset { get; set; }


        public ICollection<Cuenta> Cuentas { get; set; }
    }
}

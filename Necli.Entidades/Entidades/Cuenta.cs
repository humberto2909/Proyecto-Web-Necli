using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Necli.Entidades.Entidades
{
    public class Cuenta
    {
        [Key]
        public int IdCuenta { get; set; }
        public required string NombreTitular { get; set; }
        public required DateTime FechaCreacion { get; set; }
        public required decimal Saldo { get; set; }
        public required string Telefono { get; set; }
        public required int UsuarioId { get; set; }
        public bool EsConfirmada { get; set; } = false;
        public string? TokenConfirmacion { get; set; }


        public Usuario Usuario { get; set; }
        public ICollection<Transaccion> TransaccionesOrigen { get; set; }
        public ICollection<Transaccion> TransaccionesDestino { get; set; }
    }
}

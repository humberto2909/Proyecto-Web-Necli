using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Necli.Entidades.Entidades
{
    public class Transaccion
    {
        [Key]
        public int IdTransaccion { get; set; }
        public required DateTime FechaTransaccion { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Monto { get; set; }

        public required string TipoTransaccion { get; set; }

        [Required]
        public int CuentaOrigenId { get; set; }
        public Cuenta CuentaOrigen { get; set; }

        [Required]
        public int CuentaDestinoId { get; set; }
        public Cuenta CuentaDestino { get; set; }
    }
}

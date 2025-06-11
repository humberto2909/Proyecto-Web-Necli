using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Necli.Logica.Dtos
{
    public class CrearTransaccionExternaDto
    {
        public string TipoDocumento { get; set; }
        public string NumeroDocumento { get; set; }
        public string NumeroCuenta { get; set; }
        public string BancoId { get; set; }
        public decimal Monto { get; set; }
        public string Moneda { get; set; }
    }
}

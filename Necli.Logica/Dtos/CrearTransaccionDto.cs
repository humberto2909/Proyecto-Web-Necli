using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Necli.Logica.Dtos
{
    public class CrearTransaccionDto
    {
        public string TelefonoDestino { get; set; }        
        public decimal Monto { get; set; }
    }
}

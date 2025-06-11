using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Necli.Logica.Interface
{
    public interface ICorreoService
    {
        void EnviarCorreo(string destinatario, string asunto, string cuerpoHtml, string rutaAdjunto);
        void EnviarConAdjunto(string destino, string asunto, string contenidoHtml, string rutaAdjunto);

    }
}

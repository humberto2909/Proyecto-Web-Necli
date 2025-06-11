using System.Net;
using System.Net.Mail;
using Necli.Persistencia.Utils;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using Necli.Logica.Interface;

namespace Necli.Logica.Service
{
    public class CorreoService : ICorreoService
    {
        private readonly IConfiguration _config;
        private readonly SmtpSettings _smtpSettings;

        public CorreoService(IConfiguration config, IOptions<SmtpSettings> smtpOptions)
        {
            _config = config;
            _smtpSettings = smtpOptions.Value;
        }

        public void EnviarCorreo(string destinatario, string asunto, string htmlCuerpo, string rutaAdjunto)
        {
            try
            {
                var smtp = _config.GetSection("SmtpSettings"); // Asegúrate de usar el nombre correcto

                var remitente = smtp["Remitente"];
                var user = smtp["User"];
                var password = smtp["Password"];
                var host = smtp["Host"];
                var port = int.Parse(smtp["Port"]);
                var enableSsl = bool.Parse(smtp["EnableSsl"]);

                // Validaciones básicas
                if (string.IsNullOrWhiteSpace(remitente) ||
                    string.IsNullOrWhiteSpace(user) ||
                    string.IsNullOrWhiteSpace(password) ||
                    string.IsNullOrWhiteSpace(destinatario) ||
                    string.IsNullOrWhiteSpace(asunto) ||
                    string.IsNullOrWhiteSpace(htmlCuerpo))
                {
                    throw new ArgumentException("Faltan datos necesarios para enviar el correo.");
                }

                var mensaje = new MailMessage(remitente, destinatario, asunto, htmlCuerpo)
                {
                    IsBodyHtml = true
                };

                // Validar y adjuntar archivo
                if (!string.IsNullOrWhiteSpace(rutaAdjunto))
                {
                    if (File.Exists(rutaAdjunto))
                    {
                        mensaje.Attachments.Add(new Attachment(rutaAdjunto));
                    }
                    else
                    {
                        throw new FileNotFoundException("El archivo adjunto no se encontró.", rutaAdjunto);
                    }
                }

                using var cliente = new SmtpClient(host, port)
                {
                    Credentials = new NetworkCredential(user, password),
                    EnableSsl = enableSsl
                };

                cliente.Send(mensaje);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al enviar correo: {ex}");
                throw; // Permite ver el error real en la capa que lo llame
            }
        }




        public void EnviarConAdjunto(string destino, string asunto, string contenidoHtml, string rutaAdjunto)
        {
            Console.WriteLine("📤 Remitente que se usará: " + _smtpSettings.Remitente);

            if (string.IsNullOrWhiteSpace(_smtpSettings.Remitente))
                throw new Exception("El campo 'Remitente' está vacío.");

            var mensaje = new MailMessage
            {
                From = new MailAddress(_smtpSettings.Remitente),
                Subject = asunto,
                Body = contenidoHtml,
                IsBodyHtml = true
            };

            mensaje.To.Add(destino);

            if (!string.IsNullOrWhiteSpace(rutaAdjunto) && File.Exists(rutaAdjunto))
            {
                mensaje.Attachments.Add(new Attachment(rutaAdjunto));
            }

            using var smtp = new SmtpClient(_smtpSettings.Host, _smtpSettings.Port)
            {
                EnableSsl = _smtpSettings.EnableSsl,
                Credentials = new NetworkCredential(_smtpSettings.User, _smtpSettings.Password)
            };

            smtp.Send(mensaje);
        }
    }
}


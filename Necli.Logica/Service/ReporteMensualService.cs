using Necli.Entidades.Entidades;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;


namespace Necli.Logica.Service
{
    public class ReporteMensualService
    {
        public string GenerarPdfProtegido(Cuenta cuenta, List<Transaccion> transacciones)
        {
            var usuario = cuenta.Usuario;
            var cedula = usuario.Cedula;

            // Obtener año y mes de la primera transacción
            var primeraTransaccion = transacciones.OrderBy(t => t.FechaTransaccion).First();
            var año = primeraTransaccion.FechaTransaccion.Year;
            var mes = primeraTransaccion.FechaTransaccion.Month;

            // Construir la ruta del directorio
            var rutaBase = Path.Combine(
                Directory.GetParent(Directory.GetCurrentDirectory())!.FullName,
                "Reportes"
            );
            var directorio = Path.Combine(
                rutaBase,
                año.ToString(),
                mes.ToString("D2")
            );

            Console.WriteLine($"📁 Ruta destino del PDF: {directorio}");

            try
            {
                Directory.CreateDirectory(directorio);
                Console.WriteLine($"✅ Directorio creado (o ya existente): {directorio}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error creando directorio: {ex.Message}");
                return null;
            }

            // Timestamp para el nombre del archivo
            var timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
            var nombreArchivo = $"{cedula}_{timestamp}.pdf";
            var rutaArchivo = Path.Combine(directorio, nombreArchivo);

            try
            {
                using (var fs = new FileStream(rutaArchivo, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    var document = new iTextSharp.text.Document();
                    var writer = PdfWriter.GetInstance(document, fs);

                    writer.SetEncryption(
                        Encoding.UTF8.GetBytes(cedula), // contraseña de apertura
                        null,                           // sin contraseña de propietario
                        PdfWriter.ALLOW_PRINTING,
                        PdfWriter.ENCRYPTION_AES_128
                    );

                    document.Open();
                    document.Add(new Paragraph($"Movimientos de la cuenta: {cuenta.IdCuenta}"));
                    document.Add(new Paragraph($"Titular: {usuario.NombreUsuario} {usuario.ApellidoUsuario}"));
                    document.Add(new Paragraph(" "));

                    foreach (var t in transacciones)
                    {
                        document.Add(new Paragraph(
                            $"{t.FechaTransaccion:yyyy-MM-dd} | {t.TipoTransaccion.ToUpper()} | ${t.Monto} → Cuenta {t.CuentaDestinoId}"
                        ));
                    }

                    document.Close();
                }

                Console.WriteLine($"PDF guardado en: {rutaArchivo}");
                return rutaArchivo;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generando PDF: {ex.Message}");
                return null;
            }
        }

    }
}

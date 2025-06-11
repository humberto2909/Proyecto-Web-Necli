using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;
using Necli.Logica.Interface;

public class ReporteProgramadoService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public ReporteProgramadoService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var ahora = DateTime.Now;

            // Si hoy es día 1, ejecutamos el reporte
            if (ahora.Day == 1)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var transaccionService = scope.ServiceProvider.GetRequiredService<ITransaccionService>();
                    transaccionService.GenerarReportesMensuales();
                }
            }

            // Esperar 24 horas antes de volver a revisar
            await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
        }
    }

}


using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using BaresTucuman.API.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BaresTucuman.API.Workers
{
    public class BarSyncWorker : BackgroundService
    {
        private readonly ILogger<BarSyncWorker> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly double _minutosIntervalo;

        public BarSyncWorker(ILogger<BarSyncWorker> logger, IServiceProvider serviceProvider, IConfiguration config)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;

            var intervalString = config["SyncWorker: IntervalMinutes"];
            if (string.IsNullOrWhiteSpace(intervalString))
            {
                throw new ArgumentNullException("Falta el intervalo de sincronización en minutos en la configuración");
            }

            if (!double.TryParse(intervalString, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out _minutosIntervalo))
            {
                throw new FormatException($"Valor inválido para 'SyncWorker: IntervalMinutes': '{intervalString}'");
            }

            if (_minutosIntervalo <= 0)
            {
                throw new ArgumentOutOfRangeException("SyncWorker: IntervalMinutes", "El intervalo de sincronización debe ser mayor a cero.");
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("El Worker de sincronización de bares ha iniciado.");

            using var timer = new PeriodicTimer(TimeSpan.FromMinutes(_minutosIntervalo));

            try
            {
                await DoWorkAsync();

                while (await timer.WaitForNextTickAsync(stoppingToken))
                {
                    await DoWorkAsync();
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("El Worker se está deteniendo.");
            }
        }

        private async Task DoWorkAsync()
        {
            try
            {
                _logger.LogInformation("Iniciando sincronización de datos externa...");

                using var scope = _serviceProvider.CreateScope();
                var syncService = scope.ServiceProvider.GetRequiredService<BarSyncService>();

                int agregados = await syncService.SyncBaresAsync();

                if (agregados > 0)
                {
                    _logger.LogInformation("Sincronización exitosa. Se agregaron {Count} bares nuevos a la base de datos.", agregados);
                }
                else
                {
                    _logger.LogInformation("Sincronización finalizada. No se encontraron bares nuevos (cero duplicados).");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocurrió un error crítico durante la sincronización.");
            }
        }
    }
}
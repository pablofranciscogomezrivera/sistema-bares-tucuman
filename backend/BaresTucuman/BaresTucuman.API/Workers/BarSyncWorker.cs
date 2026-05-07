using BaresTucuman.API.Services;

namespace BaresTucuman.API.Workers
{
    public class BarSyncWorker : BackgroundService
    {
        private readonly ILogger<BarSyncWorker> _logger;
        private readonly IServiceProvider _serviceProvider;

        public BarSyncWorker(ILogger<BarSyncWorker> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("El Worker de sincronización de bares ha iniciado.");

            //TimeSpan.FromHours(24).
            using var timer = new PeriodicTimer(TimeSpan.FromMinutes(1));

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
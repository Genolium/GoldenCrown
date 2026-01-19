using GoldenCrown.Data;
using Microsoft.EntityFrameworkCore;

namespace GoldenCrown.Services
{
    // BackgroundService создается один раз при запуске приложения и живет вечно.
    public class SessionCleanupService : BackgroundService
    {
        // Фабрика, чтобы создавать одноразовые области (scopes)
        private readonly IServiceScopeFactory _scopeFactory;

        // Логгер, чтобы писать информацию в консоль.
        private readonly ILogger<SessionCleanupService> _logger;

        public SessionCleanupService(
            IServiceScopeFactory scopeFactory,
            ILogger<SessionCleanupService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        // Этот метод вызывается автоматически при старте приложения.
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Фоновая служба очистки сессий ЗАПУЩЕНА");

            // Бесконечный цикл, пока работает сервер.
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Начинаю проверку истекших сессий...");
                                     
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        // Фабрика создает новый экземпляр контекста
                        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                                                
                        int deletedCount = await context.Sessions
                            .Where(s => s.ExpiresAt < DateTime.UtcNow) // Cрок действия меньше "сейчас"
                            .ExecuteDeleteAsync(stoppingToken);        // Передаем токен отмены

                        if (deletedCount > 0)
                        {
                            _logger.LogInformation("УСПЕХ: Удалено {Count} старых сессий.", deletedCount);
                        }
                        else
                        {
                            _logger.LogInformation("Чисто: Истекших сессий не найдено.");
                        }

                    } // Scope умирает. Соединение с БД закрывается
                }
                catch (Exception ex)
                {                   
                    _logger.LogError(ex, "КРИТИЧЕСКАЯ ОШИБКА в фоновой службе SessionCleanupService");
                }
                                
                await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
            }

            _logger.LogInformation("Фоновая служба очистки сессий ОСТАНОВЛЕНА");
        }
    }
}
using ErrSendApplication.DTO;
using ErrSendApplication.Interfaces;
using System.Text.Json;

namespace ErrSendWebApi.Middleware
{
    public class TelegramErrorMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger<TelegramErrorMiddleware> logger;
        private readonly IServiceScopeFactory serviceScopeFactory;

        public TelegramErrorMiddleware(
            RequestDelegate next,
            ILogger<TelegramErrorMiddleware> logger,
            IServiceScopeFactory serviceScopeFactory)
        {
            this.next = next;
            this.logger = logger;
            this.serviceScopeFactory = serviceScopeFactory;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                
                // Відправляємо помилку в Telegram в background
                _ = Task.Run(async () => await SendErrorToTelegramAsync(ex, context));
                
                throw; // Повторне перекидання для збереження нормального потоку обробки помилок
            }
        }

        private async Task SendErrorToTelegramAsync(Exception exception, HttpContext context)
        {
        
                using var scope = serviceScopeFactory.CreateScope();
                var telegramService = scope.ServiceProvider.GetService<ITelegramService>();
                
                if (telegramService == null)
                {
                    return;
                }

                var errorReport = new ErrorReportDto
                {
                    ErrorMessage = exception.Message,
                    Source = $"{context.Request.Method} {context.Request.Path}",
                    StackTrace = exception.StackTrace ?? "",
                    Severity = "Error",
                    UserId = context.User?.Identity?.Name ?? "Анонім",
                    AdditionalInfo = $"User-Agent: {context.Request.Headers.UserAgent}\nRemote IP: {context.Connection.RemoteIpAddress}"
                };

                var result = await telegramService.SendErrorAsync(new Domain.Models.ErrorReport
                {
                    ErrorMessage = errorReport.ErrorMessage,
                    Source = errorReport.Source,
                    StackTrace = errorReport.StackTrace,
                    Severity = errorReport.Severity,
                    UserId = errorReport.UserId,
                    AdditionalInfo = errorReport.AdditionalInfo,
                    Timestamp = DateTime.UtcNow
                });
                
        }
    }
} 
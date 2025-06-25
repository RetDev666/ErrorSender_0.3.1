using ErrSendApplication.Interfaces;
using MediatR;
using Serilog;

namespace ErrSendApplication.Behaviors
{
    /// <summary>
    /// Клас для поведінки логування через Serilog.
    /// </summary>
    /// <typeparam name="TRequest">Запит</typeparam>
    /// <typeparam name="TResponse">Відповідь</typeparam>
    public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        ICurrentService currentService;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="currentUserService">Поточний сервіс</param>
        public LoggingBehavior(ICurrentService currentUserService)
        {
            this.currentService = currentUserService;
        }

        /// <summary>
        /// Метод обробки логування у файл.
        /// </summary>
        /// <param name="request">Запит</param>
        /// <param name="next">Делегат що переключає наступну відповідь.</param>
        /// <param name="cancellationToken">Токен відміни</param>
        /// <returns>Відповідь</returns>
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var requestName = typeof(TRequest).Name;
            var userName = currentService.GetCurrentUserName() ?? "Anonymous";
            var userId = currentService.GetCurrentUserId() ?? "Unknown";
            
            Log.Information("Request: {Name} | User: {UserName} ({UserId}) | {@Request}", 
                requestName, userName, userId, request);

            var startTime = DateTime.UtcNow;
            var response = await next();
            var duration = DateTime.UtcNow - startTime;

            Log.Information("Request completed: {Name} | Duration: {Duration}ms", 
                requestName, duration.TotalMilliseconds);

            return response;
        }
    }
}

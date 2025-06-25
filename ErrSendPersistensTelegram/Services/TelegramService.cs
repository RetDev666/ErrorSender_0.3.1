using Domain.Models;
using ErrSendApplication.Common.Configs;
using ErrSendApplication.DTO;
using ErrSendApplication.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;

namespace ErrSendPersistensTelegram.Services
{
    public class TelegramService : ITelegramService
    {
        private readonly IHttpClientWr httpClient;
        private readonly TelegramConfig config;
        private readonly ILogger<TelegramService> logger;

        public TelegramService(
            IHttpClientWr httpClient,
            IOptions<TelegramConfig> config,
            ILogger<TelegramService> logger)
        {
            this.httpClient = httpClient;
            this.config = config.Value;
            this.logger = logger;
        }

        public async Task<SendErrorToTelegramResponse> SendErrorAsync(ErrorReport errorReport)
        {
            try
            {
                var message = FormatErrorMessage(errorReport);
                var telegramMessage = new
                {
                    chat_id = config.ChatId,
                    text = message,
                    parse_mode = "HTML"
                };

                var json = JsonSerializer.Serialize(telegramMessage);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var url = $"{config.BaseUrl}{config.BotToken}/sendMessage";
                
                var response = await httpClient.PostAsync(url, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var telegramResponse = JsonSerializer.Deserialize<TelegramApiResponse>(responseContent);
                    
                    return new SendErrorToTelegramResponse
                    {
                        IsSuccess = true,
                        TelegramMessageId = telegramResponse?.Result?.MessageId.ToString() ?? ""
                    };
                }
                else
                {
                    return new SendErrorToTelegramResponse
                    {
                        IsSuccess = false,
                    };
                }
            }
            catch (Exception ex)
            {
                return new SendErrorToTelegramResponse
                {
                    IsSuccess = false,
                };
            }
        }

        private string FormatErrorMessage(ErrorReport errorReport)
        {
            var emoji = errorReport.Severity switch
            {
                "Error" => "🔴",
                "Warning" => "🟡",
                "Info" => "🔵",
                _ => "⚪"
            };

            var message = new StringBuilder();
            message.AppendLine($"{emoji} <b>ЗВІТ ПРО ПОМИЛКУ</b>");
            message.AppendLine($"<b>Тяжкість:</b> {errorReport.Severity}");
            message.AppendLine($"<b>Час:</b> {errorReport.Timestamp:yyyy-MM-dd HH:mm:ss} UTC");
            message.AppendLine($"<b>Джерело:</b> {errorReport.Source}");
            
            if (!string.IsNullOrEmpty(errorReport.UserId))
                message.AppendLine($"<b>Користувач:</b> {errorReport.UserId}");
            
            message.AppendLine($"<b>Повідомлення:</b> {errorReport.ErrorMessage}");
            
            if (!string.IsNullOrEmpty(errorReport.AdditionalInfo))
                message.AppendLine($"<b>Додаткова інформація:</b> {errorReport.AdditionalInfo}");
            
            if (!string.IsNullOrEmpty(errorReport.StackTrace))
            {
                var truncatedStackTrace = errorReport.StackTrace.Length > 1000 
                    ? errorReport.StackTrace.Substring(0, 1000) + "..."
                    : errorReport.StackTrace;
                message.AppendLine($"<b>Трасування стека:</b>\n<code>{truncatedStackTrace}</code>");
            }

            return message.ToString();
        }
    }

    public class TelegramApiResponse
    {
        public bool Ok { get; set; }
        public TelegramMessage? Result { get; set; }
    }

    public class TelegramMessage
    {
        public int MessageId { get; set; }
    }
} 
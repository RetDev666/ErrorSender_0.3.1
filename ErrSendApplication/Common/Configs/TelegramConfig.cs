namespace ErrSendApplication.Common.Configs
{
    public class TelegramConfig
    {
        public required string BotToken { get; set; } 
        public required string ChatId { get; set; } 
        public string BaseUrl { get; set; } = "https://api.telegram.org/bot";
    }
} 
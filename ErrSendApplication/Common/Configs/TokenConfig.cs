namespace ErrSendApplication.Common.Configs
{
    public class TokenConfig
    {
        public required string TokenKey { get; set; } 
        public required string Issuer { get; set; }
        public required string Audience { get; set; } 
        public int ExpiryInMinutes { get; set; } = 60;
    }
} 
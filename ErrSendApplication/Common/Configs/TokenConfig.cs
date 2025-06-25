namespace ErrSendApplication.Common.Configs
{
    public class TokenConfig
    {
        public string TokenKey { get; set; } 
        public string Issuer { get; set; }
        public string Audience { get; set; } 
        public int ExpiryInMinutes { get; set; }
    }
} 
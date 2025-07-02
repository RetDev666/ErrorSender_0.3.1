namespace Domain.Models
{
    public class ErrorReport
    {
        public int Id { get; set; }
        public required string ErrorMessage { get; set; }
        public required string Source { get; set; } 
        public string? StackTrace { get; set; } 
        public DateTime Timestamp { get; set; }
        public string? UserId { get; set; } 
        public string Severity { get; set; } = "Error";
        public string? AdditionalInfo { get; set; } 
    }
} 
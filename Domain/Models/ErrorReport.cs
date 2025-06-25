namespace Domain.Models
{
    public class ErrorReport
    {
        public int Id { get; set; }
        public string ErrorMessage { get; set; }
        public string Source { get; set; } 
        public string StackTrace { get; set; } 
        public DateTime Timestamp { get; set; }
        public string UserId { get; set; } 
        public string Severity { get; set; } 
        public string AdditionalInfo { get; set; } 
    }
} 
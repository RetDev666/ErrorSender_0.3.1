using AutoMapper;
using Domain.Models;
using ErrSendApplication.Mappings;

namespace ErrSendApplication.DTO
{
    public class ErrorReportDto : IMapWith<ErrorReport>
    {
        public string ErrorMessage { get; set; } 
        public string Source { get; set; } 
        public string StackTrace { get; set; } 
        public string UserId { get; set; } 
        public string Severity { get; set; } 
        public string AdditionalInfo { get; set; } 

        public void Mapping(Profile profile)
        {
            profile.CreateMap<ErrorReportDto, ErrorReport>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Timestamp, opt => opt.Ignore());

            profile.CreateMap<ErrorReport, ErrorReportDto>();
        }
    }
    
    public class SendErrorToTelegramResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } 
        public string TelegramMessageId { get; set; } 
    }
} 
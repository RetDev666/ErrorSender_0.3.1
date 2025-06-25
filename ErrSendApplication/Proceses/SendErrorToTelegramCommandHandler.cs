using AutoMapper;
using Domain.Models;
using ErrSendApplication.DTO;
using ErrSendApplication.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ErrSendApplication.Proceses
{
    public class SendErrorToTelegramCommandHandler : IRequestHandler<SendErrorToTelegramCommand, SendErrorToTelegramResponse>
    {
        private readonly ITelegramService telegramService;
        private readonly IMapper mapper;
        private readonly ILogger<SendErrorToTelegramCommandHandler> logger;

        public SendErrorToTelegramCommandHandler(
            ITelegramService telegramService,
            IMapper mapper,
            ILogger<SendErrorToTelegramCommandHandler> logger)
        {
            this.telegramService = telegramService;
            this.mapper = mapper;
            this.logger = logger;
        }

        public async Task<SendErrorToTelegramResponse> Handle(SendErrorToTelegramCommand request, CancellationToken cancellationToken)
        {
            try
            {

                var errorReport = mapper.Map<ErrorReport>(request.ErrorReport);
                errorReport.Timestamp = DateTime.UtcNow;

                var result = await telegramService.SendErrorAsync(errorReport);

                return result;
            }
            catch (Exception ex)
            {
                return new SendErrorToTelegramResponse
                {
                    IsSuccess = false,
                };
            }
        }
    }
} 
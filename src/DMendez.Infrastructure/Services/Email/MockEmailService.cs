using System.Threading;
using System.Threading.Tasks;
using DMendez.Application.Interfaces.External;
using Microsoft.Extensions.Logging;

namespace DMendez.Infrastructure.Services.Email
{
    public class MockEmailService : IEmailService
    {
        private readonly ILogger<MockEmailService> _logger;

        public MockEmailService(ILogger<MockEmailService> logger)
        {
            _logger = logger;
        }

        public Task SendEmailAsync(string to, string subject, string body, bool isHtml = true, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("================ [MOCK EMAIL ENVIADO] ================");
            _logger.LogInformation("Para: {To}", to);
            _logger.LogInformation("Asunto: {Subject}", subject);
            _logger.LogInformation("Formato: {Format}", isHtml ? "HTML" : "Texto plano");
            _logger.LogInformation("Cuerpo:\n{Body}", body);
            _logger.LogInformation("=======================================================");

            return Task.CompletedTask;
        }
    }
}

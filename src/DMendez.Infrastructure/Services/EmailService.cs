using System.Threading;
using System.Threading.Tasks;
using DMendez.Application.Interfaces.External;
using Microsoft.Extensions.Logging;

namespace DMendez.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;

        public EmailService(ILogger<EmailService> logger)
        {
            _logger = logger;
        }

        public Task SendEmailAsync(string to, string subject, string body, bool isHtml = true, CancellationToken cancellationToken = default)
        {
            // En desarrollo, logueamos el correo. Se puede conectar con SMTP o SendGrid según configuración.
            _logger.LogInformation("Enviando correo a {To} con asunto '{Subject}'", to, subject);
            return Task.CompletedTask;
        }
    }
}

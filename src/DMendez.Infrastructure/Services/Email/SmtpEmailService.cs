using System;
using System.Threading;
using System.Threading.Tasks;
using DMendez.Application.Interfaces.External;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace DMendez.Infrastructure.Services.Email
{
    public class SmtpEmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<SmtpEmailService> _logger;

        public SmtpEmailService(IConfiguration configuration, ILogger<SmtpEmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendEmailAsync(string to, string subject, string body, bool isHtml = true, CancellationToken cancellationToken = default)
        {
            var host = _configuration["EmailSettings:Host"] ?? "smtp.gmail.com";
            var port = int.Parse(_configuration["EmailSettings:Port"] ?? "587");
            var user = _configuration["EmailSettings:User"] ?? "";
            var pass = _configuration["EmailSettings:Password"] ?? "";
            var from = _configuration["EmailSettings:From"] ?? user;

            try
            {
                var message = new MimeMessage();
                message.From.Add(MailboxAddress.Parse(from));
                message.To.Add(MailboxAddress.Parse(to));
                message.Subject = subject;

                var bodyFormat = isHtml ? "html" : "plain";
                message.Body = new TextPart(bodyFormat) { Text = body };

                using var client = new SmtpClient();
                // Timeout de 5000ms para evitar cuelgues si los puertos de salida SMTP están bloqueados por el hosting
                client.Timeout = 5000;

                var secureOption = port == 465
                    ? SecureSocketOptions.SslOnConnect
                    : SecureSocketOptions.StartTls;

                _logger.LogInformation("[Email] Conectando a servidor SMTP {Host}:{Port}...", host, port);

                await client.ConnectAsync(host, port, secureOption, cancellationToken);

                if (!string.IsNullOrWhiteSpace(user) && !string.IsNullOrWhiteSpace(pass))
                {
                    await client.AuthenticateAsync(user, pass, cancellationToken);
                }

                await client.SendAsync(message, cancellationToken);
                await client.DisconnectAsync(true, cancellationToken);

                _logger.LogInformation("[Email] Correo enviado exitosamente vía SMTP a {To}", to);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Email] Error al enviar correo vía SMTP a {To}", to);
                throw;
            }
        }
    }
}

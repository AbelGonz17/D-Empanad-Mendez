using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using DMendez.Application.Interfaces.External;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DMendez.Infrastructure.Services.Email
{
    public class ResendEmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<ResendEmailService> _logger;
        private readonly HttpClient _httpClient;

        public ResendEmailService(
            IConfiguration configuration,
            ILogger<ResendEmailService> logger,
            IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient("Resend");
        }

        public async Task SendEmailAsync(string to, string subject, string body, bool isHtml = true, CancellationToken cancellationToken = default)
        {
            var apiKey = _configuration["EmailSettings:ResendApiKey"];
            var from = _configuration["EmailSettings:From"] ?? "onboarding@resend.dev";

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                _logger.LogWarning("[Email] ResendApiKey no configurado. El correo a {To} no será enviado.", to);
                return;
            }

            object payload = isHtml
                ? new { from, to = new[] { to }, subject, html = body }
                : new { from, to = new[] { to }, subject, text = body };

            var json = JsonSerializer.Serialize(payload);
            using var request = new HttpRequestMessage(HttpMethod.Post, "https://api.resend.com/emails")
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            _logger.LogInformation("[Email] Enviando correo vía Resend a {To} con asunto: '{Subject}'", to, subject);

            try
            {
                var response = await _httpClient.SendAsync(request, cancellationToken);
                var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("[Email] Correo enviado exitosamente vía Resend a {To}. Respuesta: {Response}", to, responseBody);
                }
                else
                {
                    _logger.LogError("[Email] Error al enviar correo a {To}. Código de estado: {StatusCode}. Respuesta: {Body}", to, response.StatusCode, responseBody);
                    throw new InvalidOperationException($"Error en API de Resend: {response.StatusCode} - {responseBody}");
                }
            }
            catch (Exception ex) when (ex is not InvalidOperationException)
            {
                _logger.LogError(ex, "[Email] Excepción al comunicar con la API de Resend para el correo a {To}", to);
                throw;
            }
        }
    }
}

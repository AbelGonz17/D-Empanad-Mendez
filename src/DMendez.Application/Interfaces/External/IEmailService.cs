using System.Threading;
using System.Threading.Tasks;

namespace DMendez.Application.Interfaces.External
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body, bool isHtml = true, CancellationToken cancellationToken = default);
    }
}

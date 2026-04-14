using Decorator_Implementation.Models;

namespace Decorator_Implementation.Interfaces
{
    public interface IEmailService
    {
        EmailResult Send(EmailMessage emailMessage);
    }
}

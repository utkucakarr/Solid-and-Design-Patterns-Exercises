using Decorator_Implementation.Interfaces;
using Decorator_Implementation.Models;

namespace Decorator_Implementation.Decorators
{
    // Abstract Decorator — tüm decorator'ların base class'ı
    // IEmailService'i implement eder ve bir IEmailService sarar
    public abstract class BaseEmailDecorator : IEmailService
    {
        protected readonly IEmailService _innerService;

        protected BaseEmailDecorator(IEmailService innerService)
        {
            _innerService = innerService
                ?? throw new ArgumentNullException(nameof(innerService));
        }

        // Default olarak inner servise ilet — alt sınıflar override eder
        public virtual EmailResult Send(EmailMessage emailMessage)
            => _innerService.Send(emailMessage);
    }
}

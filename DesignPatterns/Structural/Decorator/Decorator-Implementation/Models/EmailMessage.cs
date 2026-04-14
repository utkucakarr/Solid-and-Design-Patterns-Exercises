namespace Decorator_Implementation.Models
{
    public class EmailMessage
    {
        public string To { get; }
        public string Subject { get; }
        public string Body { get; }

        public EmailMessage(string to, string subject, string body)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(to, nameof(to));
            ArgumentException.ThrowIfNullOrWhiteSpace(subject, nameof(subject));
            ArgumentException.ThrowIfNullOrWhiteSpace(body, nameof(body));

            To = to;
            Subject = subject;
            Body = body;
        }
    }
}

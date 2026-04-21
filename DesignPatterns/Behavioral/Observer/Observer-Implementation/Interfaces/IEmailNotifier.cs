namespace Observer_Implementation.Interfaces
{
    public interface IEmailNotifier
    {
        void Send(string to, string subject, string body);
    }
}

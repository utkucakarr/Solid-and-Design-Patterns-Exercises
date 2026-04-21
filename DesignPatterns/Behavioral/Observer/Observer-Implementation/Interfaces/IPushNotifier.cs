namespace Observer_Implementation.Interfaces
{
    public interface IPushNotifier
    {
        void Send(string deviceToken, string title, string body);
    }
}

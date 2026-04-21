namespace Observer_Implementation.Interfaces
{
    public interface ISmsNotifier
    {
        void Send(string phoneNumber, string message);
    }
}

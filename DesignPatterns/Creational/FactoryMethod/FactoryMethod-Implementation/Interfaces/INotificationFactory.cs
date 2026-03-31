namespace FactoryMethod_Implementation.Interfaces
{
    // Factory Method sözleşmesi — hangi nesnenin
    // oluşturulacağını alt sınıflar belirler
    public interface INotificationFactory
    {
        INotification CreateNotification();
    }
}

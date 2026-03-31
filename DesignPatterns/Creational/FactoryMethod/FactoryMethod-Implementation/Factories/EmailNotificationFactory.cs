using FactoryMethod_Implementation.Interfaces;
using FactoryMethod_Implementation.Services;

namespace FactoryMethod_Implementation.Factories
{
    public class EmailNotificationFactory : INotificationFactory
    {
        // Nesne oluşturma sorumluluğu burada — client bilmek zorunda değil
        public INotification CreateNotification()
            => new EmailNotification();
    }
}

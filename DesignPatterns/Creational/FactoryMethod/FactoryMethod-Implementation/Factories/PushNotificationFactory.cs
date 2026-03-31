using FactoryMethod_Implementation.Interfaces;
using FactoryMethod_Implementation.Services;

namespace FactoryMethod_Implementation.Factories
{
    public class PushNotificationFactory : INotificationFactory
    {
        public INotification CreateNotification()
            => new PushNotification();
    }
}

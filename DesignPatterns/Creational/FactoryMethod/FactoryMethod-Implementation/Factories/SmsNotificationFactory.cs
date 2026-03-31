using FactoryMethod_Implementation.Interfaces;
using FactoryMethod_Implementation.Services;

namespace FactoryMethod_Implementation.Factories
{
    public class SmsNotificationFactory : INotificationFactory
    {
        public INotification CreateNotification()
            => new SmsNotification();
    }
}

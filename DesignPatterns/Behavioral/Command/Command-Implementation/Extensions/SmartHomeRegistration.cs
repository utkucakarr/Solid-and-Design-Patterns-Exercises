using Command_Implementation.Devices;
using Command_Implementation.Invoker;
using Microsoft.Extensions.DependencyInjection;

namespace Command_Implementation.Extensions
{
    public static class SmartHomeRegistration
    {
        public static IServiceCollection AddSmartHomeServices(this IServiceCollection services)
        {
            // Cihazları (Receiver) fiziksel cihazlar oldukları için Singleton olarak ekliyoruz
            // (Gerçek hayatta aynı oturma odası ışığından bellekte 2 tane olamaz)
            services.AddSingleton(_ => new Light("Oturma Odası"));
            services.AddSingleton<Thermostat>();
            services.AddSingleton<SecurityCamera>();

            // Controller'ı (Invoker) da Singleton ekliyoruz ki History ve Undo/Redo stack'leri silinmesin
            services.AddSingleton<SmartHomeController>();

            return services;
        }
    }
}

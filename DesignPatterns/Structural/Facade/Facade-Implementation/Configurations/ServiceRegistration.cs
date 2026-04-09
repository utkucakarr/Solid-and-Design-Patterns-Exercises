using Facade_Implementation.Facade;
using Facade_Implementation.Interfaces;
using Facade_Implementation.Subsystems;
using Microsoft.Extensions.DependencyInjection;

namespace Facade_Implementation.Configurations
{
    // Sınıf mutlaka static olmalı
    public static class ServiceRegistration
    {
        // Method static olmalı ve ilk parametre ""this IServiceCollection ile başlamalıdır.
        public static IServiceCollection AddBankingServices(this IServiceCollection services)
        {
            // Tüm bağımlılıkların kayıt işlemlerini buraya yazıyoruz.
            services.AddScoped<IBalanceService, BalanceService>();
            services.AddScoped<IFraudService, FraudService>();
            services.AddScoped<ITransferService, TransferService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IAuditService, AuditService>();

            // Facade sınıfımızı da ekliyoruz
            services.AddScoped<BankingFacade>();

            // Zincirleme kullanım (method chaining) yapılabilmesi için services'i geri dönüyoruz
            return services;
        }
    }
}

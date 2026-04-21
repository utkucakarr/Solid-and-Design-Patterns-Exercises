using Microsoft.Extensions.DependencyInjection;
using Observer_Implementation.Interfaces;
using Observer_Implementation.Observers;
using Observer_Implementation.Subject;

namespace Observer_Implementation.Extensions
{
    public static class ObserverServiceRegistration
    {
        public static IServiceCollection AddOrderObserverService(this IServiceCollection services)
        {
            services.AddScoped<IEmailNotifier, EmailNotifier>();
            services.AddScoped<ISmsNotifier, SmsNotifier>();
            services.AddScoped<IPushNotifier, PushNotifier>();
            services.AddScoped<IInventoryService, InventoryService>();
            services.AddScoped<IInvoiceService, InvoiceService>();

            services.AddScoped<EmailNotificationObserver>();
            services.AddScoped<SmsNotificationObserver>();
            services.AddScoped<PushNotificationObserver>();
            services.AddScoped<InventoryObserver>();
            services.AddScoped<InvoiceObserver>();

            services.AddScoped<OrderService>();

            return services;
        }
    }
}

using ChainOfResponsibility_Implementation.Handlers;
using Microsoft.Extensions.DependencyInjection;

namespace ChainOfResponsibility_Implementation.Extensions
{
    public static class OrderHandlerProvider
    {
        public static IServiceCollection AddOrderHander(this IServiceCollection services)
        {
            services.AddTransient<StockHandler>();
            services.AddTransient<FraudHandler>();
            services.AddTransient<PaymentHandler>();
            services.AddTransient<ShippingHandler>();

            return services;
        }
    }
}

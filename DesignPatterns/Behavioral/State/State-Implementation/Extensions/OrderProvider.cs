using Microsoft.Extensions.DependencyInjection;
using State_Implementation.Interfaces;
using State_Implementation.Orders;

namespace State_Implementation.Extensions
{
    public static class OrderProvider
    {
        public static IServiceCollection AddOrder(this IServiceCollection services)
        {
            services.AddTransient<IOrderFactory, OrderFactory>();

            return services;
        }
    }
}

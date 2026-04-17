using Microsoft.Extensions.DependencyInjection;
using Strategy_Implementation.Context;
using Strategy_Implementation.Interfaces;
using Strategy_Implementation.Strategies;

namespace Strategy_Implementation.Extensions
{
    public static class ShippingServiceRegistration
    {
        public static IServiceCollection AddShippingService(this IServiceCollection services)
        {
            services.AddScoped<IShippingContext, ShippingContext>();
            services.AddTransient<StandardShippingStrategy>();
            services.AddTransient<ExpressingShippingStrategy>();
            services.AddTransient<MemberShippingStrategy>();
            services.AddTransient<FreeShippingStrategy>();

            return services;
        }
    }
}

using Microsoft.Extensions.DependencyInjection;
using Visitor_Implementation.Visitors;

namespace Visitor_Implementation.Extensions
{
    public static class ProductVisitorProvider
    {
        public static IServiceCollection AddProcutVisitor(this IServiceCollection services)
        {
            services.AddTransient<TaxCalculatorVisitor>();
            services.AddTransient<ReportVisitor>();


            return services;
        }
    }
}

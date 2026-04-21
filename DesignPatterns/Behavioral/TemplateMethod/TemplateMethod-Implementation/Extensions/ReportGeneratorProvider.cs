using Microsoft.Extensions.DependencyInjection;
using TemplateMethod_Implementation.Reports;

namespace TemplateMethod_Implementation.Extensions
{
    public static class ReportGeneratorProvider
    {
        public static IServiceCollection AddReportGenerator(this IServiceCollection services)
        {
            services.AddScoped<PdfReportGenerator>();
            services.AddScoped<ExcelReportGenerator>();
            services.AddScoped<CsvReportGenerator>();

            return services;
        }
    }
}

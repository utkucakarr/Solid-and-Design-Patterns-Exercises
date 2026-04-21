using Memento_Implementation.Caretaker;
using Memento_Implementation.Interfaces;
using Memento_Implementation.Orginator;
using Microsoft.Extensions.DependencyInjection;

namespace Memento_Implementation.Extensions
{
    public static class DocumentHistroyRegistration
    {
        public static IServiceCollection AddDocumnetHistroy(this IServiceCollection services)
        {
            services.AddScoped<DocumentHistory>();
            services.AddScoped<IDocument>(sp =>
                new Document("Yeni Döküman"));
            services.AddScoped<IDocumentEditor, DocumentEditor>();

            return services;
        }
    }
}

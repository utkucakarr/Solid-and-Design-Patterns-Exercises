using Mediator_Implementation.Colleagues;
using Mediator_Implementation.Interfaces;
using Mediator_Implementation.Mediator;
using Microsoft.Extensions.DependencyInjection;

namespace Mediator_Implementation.Extensions
{
    public static class ChatRoomRegistration
    {
        public static IServiceCollection AddChatRoom(this IServiceCollection services)
        {
            services.AddScoped<IChatMediator>(sp => new ChatRoom("Genel Sohbet"));
            services.AddTransient<IAdminUser>(sp => new AdminUser("admin_ayse"));
            services.AddTransient<IRegularUser>(sp => new RegularUser("burak"));
            services.AddTransient<IBotUser>(sp => new BotUser("helper_bot"));

            return services;
        }
    }
}

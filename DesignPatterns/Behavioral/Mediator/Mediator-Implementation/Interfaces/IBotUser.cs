using Mediator_Implementation.Models;

namespace Mediator_Implementation.Interfaces
{
    public interface IBotUser : IUser
    {
        // Bot aksiyonları — otomatik yanıt mantığı Mediator üzerinden
        ChatResult SendAutoReply(string receiverUsername, string content);
    }
}

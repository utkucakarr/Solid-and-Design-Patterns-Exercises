using Mediator_Implementation.Models;

namespace Mediator_Implementation.Interfaces
{
    public interface IAdminUser : IUser
    {
        // Admin aksiyonları — ekstra yetkiler Mediator üzerinden kullanılır
        ChatResult SendMessage(string content);
        ChatResult SendPrivateMessage(string receiverUsername, string content);
        ChatResult Broadcast(string content);
        ChatResult KickUser(string targetUsername);
        ChatResult LeaveRoom();
    }
}

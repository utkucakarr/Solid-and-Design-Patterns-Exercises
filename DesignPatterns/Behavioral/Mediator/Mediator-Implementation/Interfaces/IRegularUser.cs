using Mediator_Implementation.Models;

namespace Mediator_Implementation.Interfaces
{
    public interface IRegularUser : IUser
    {
        // Regular user aksiyonları — Mediator üzerinden iletişim kurar
        ChatResult SendMessage(string content);
        ChatResult SendPrivateMessage(string receiverUsername, string content);
        ChatResult LeaveRoom();
    }
}

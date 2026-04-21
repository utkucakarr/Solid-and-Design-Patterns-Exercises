using Mediator_Implementation.Models;
using System.Collections.Generic;

namespace Mediator_Implementation.Interfaces
{
    public interface IChatMediator
    {
        // Mediator — tüm iletişimi tek noktadan koordine eder
        void Register(IUser user);
        void Unregister(IUser user);
        ChatResult SendMessage(string senderUsername, string content);
        ChatResult SendPrivateMessage(string senderUsername,
            string receiverUsername, string content);
        ChatResult Broadcast(string senderUsername, string content);
        ChatResult KickUser(string adminUsername, string targetUsername);
        IReadOnlyList<IUser> GetActiveUsers();
    }
}

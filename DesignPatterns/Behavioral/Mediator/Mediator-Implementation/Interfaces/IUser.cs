using Mediator_Implementation.Models;

namespace Mediator_Implementation.Interfaces
{
    public interface IUser
    {
        string Username { get; }
        UserRole Role { get; }

        // Colleague — sadece Mediator'ı bilir, diğer kullanıcıları bilmez
        void SetMediator(IChatMediator mediator);
        void ReceiveMessage(ChatMessage message);
        IReadOnlyList<ChatMessage> GetMessageHistory();
    }
}

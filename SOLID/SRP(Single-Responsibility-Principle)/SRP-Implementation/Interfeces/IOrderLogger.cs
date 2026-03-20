using SRP_Implementation.Models;

namespace SRP_Implementation.Interfeces
{
    public interface IOrderLogger
    {
        void LogOrderCreated(Order order);
    }
}

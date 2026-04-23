using ChainOfResponsibility_Implementation.Interfaces;
using ChainOfResponsibility_Implementation.Models;

namespace ChainOfResponsibility_Implementation.Handlers
{
    // Zincir kurma ve next çağırma mantığı tek bir base class'ta toplandı
    public abstract class BaseOrderHandler : IOrderHandler
    {
        private IOrderHandler? _next;

        public IOrderHandler SetNext(IOrderHandler next)
        {
            ArgumentNullException.ThrowIfNull(next, nameof(next));
            _next = next;
            return next; // Fluent zincir kurulumuna olanak tanır
        }

        public abstract OrderResult Handle(OrderRequest request);

        // Alt sınıflar bu metodu çağırarak zinciri devam ettirir
        protected OrderResult PassToNext(OrderRequest request)
        {
            if (_next is null)
                return OrderResult.Success($"TRK-{Guid.NewGuid().ToString()[..8].ToUpper()}");

            return _next.Handle(request);
        }
    }
}

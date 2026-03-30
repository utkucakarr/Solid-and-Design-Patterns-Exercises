using Singleton_Implementation.Interfaces;

namespace Singleton_Implementation.Services
{
    public class OrderService
    {
        private readonly IAppLogger _logger;

        public OrderService(IAppLogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void CreateOrder(string product, decimal price)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(product, nameof(product));
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(price, nameof(price));

            _logger.Info($"[OrderService] Sipariş oluşturuldu -> Ürün: {product} | Fiyat: {price} TL");
        }

        public void CancelOrder(int orderId)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(orderId, nameof(orderId));

            _logger.Warning($"[OrderService] Sipariş iptal edildi -> Sipariş ID: {orderId}");
        }
    }
}

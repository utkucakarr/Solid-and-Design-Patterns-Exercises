using Observer_Implementation.Interfaces;
using Observer_Implementation.Models;

namespace Observer_Implementation.Observers
{
    public class InventoryObserver : IOrderObserver
    {
        private readonly IInventoryService _inventoryService;

        public InventoryObserver(IInventoryService inventoryService)
        {
            ArgumentNullException.ThrowIfNull(inventoryService, nameof(inventoryService));
            _inventoryService = inventoryService;
        }

        public void OnOrderStatusChanged(Order order, OrderStatus previousStatus)
        {
            ArgumentNullException.ThrowIfNull(order, nameof(order));

            // Sipariş onaylandığında stok rezerve edilir
            if (order.Status == OrderStatus.Confirmed)
            {
                _inventoryService.ReserveStock(order.ProductName, order.Quantity);
                return;
            }

            // Sipariş iptal edildiğinde stok iade edilir
            if (order.Status == OrderStatus.Cancelled)
            {
                _inventoryService.ReleaseStock(order.ProductName, order.Quantity);
            }
        }
    }
}

using Observer_Implementation.Interfaces;
using Observer_Implementation.Models;

namespace Observer_Implementation.Observers
{
    public class InvoiceObserver : IOrderObserver
    {
        private readonly IInvoiceService _invoiceService;

        public InvoiceObserver(IInvoiceService invoiceService)
        {
            ArgumentNullException.ThrowIfNull(invoiceService, nameof(invoiceService));
            _invoiceService = invoiceService;
        }

        public void OnOrderStatusChanged(Order order, OrderStatus previousStatus)
        {
            ArgumentNullException.ThrowIfNull(order, nameof(order));

            // Fatura yalnızca sipariş teslim edildiğinde oluşturulur
            if (order.Status != OrderStatus.Delivered)
                return;

            _invoiceService.CreateInvoice(order.OrderId, order.TotalPrice);
        }
    }
}

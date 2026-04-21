namespace Observer_Implementation.Interfaces
{
    public interface IInvoiceService
    {
        void CreateInvoice(string orderId, decimal totalPrice);
    }
}

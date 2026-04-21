using Observer_Implementation.Interfaces;

namespace Observer_Implementation.Subject
{
    public class InvoiceService : IInvoiceService
    {
        // Fatura oluşturma tek sorumluluğa sahip
        public void CreateInvoice(string orderId, decimal totalPrice)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(orderId, nameof(orderId));
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(totalPrice, nameof(totalPrice));

            Console.WriteLine($" [INVOICE] {orderId} için " +
                $"{totalPrice} tutarında fatura oluşturuldu.");
        }
    }
}

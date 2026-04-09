namespace Facade_Violation.Order
{
    // Alt sistemler - client bunları doğrudan kullıyor
    public class StockServiceBad
    {
        public bool CheckStock(string productId, int quantity)
        {
            Console.WriteLine($"[Stok] {productId} için {quantity} adet kontrol ediliyor.");
            return true;
        }

        public void DecreaseStock(string productId, int quantity)
            => Console.WriteLine($"[Stok] {productId} stoku {quantity} adet düşürüldü.");
    }
}

using Observer_Implementation.Interfaces;

namespace Observer_Implementation.Subject
{
    public class InventoryService : IInventoryService
    {
        // Stok yönetimi tek sorumluluğa sahip
        public void ReserveStock(string productName, int quantity)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(productName, nameof(productName));
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quantity, nameof(quantity));

            Console.WriteLine($"  [ INVENTORY] '{productName}' için " +
                $"{quantity} adet stok rezerve edildi.");
        }

        public void ReleaseStock(string productName, int quantity)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(productName, nameof(productName));
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quantity, nameof(quantity));

            Console.WriteLine($"  [ INVENTORY] '{productName}' için " +
                $"{quantity} adet stok iade edildi.");
        }
    }
}

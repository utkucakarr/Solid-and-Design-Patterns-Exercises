namespace Observer_Implementation.Interfaces
{
    public interface IInventoryService
    {
        void ReserveStock(string productName, int quantity);
        void ReleaseStock(string productName, int quantity);
    }
}

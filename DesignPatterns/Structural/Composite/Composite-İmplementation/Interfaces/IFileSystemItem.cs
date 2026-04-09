namespace Composite_İmplementation.Interfaces
{

    // Composite sözleşmesi - hem dosya hem klasör bu interface'i imlemente eder.
    // Client dosya mı klasör mü diye sormak zorunda kalmaz!
    public interface IFileSystemItem
    {
        string Name { get; }
        long GetSize();
        void Print(string indent = "");
        IFileSystemItem? GetParent();
        void SetParent(IFileSystemItem? parent);
    }
}

namespace Memento_Implementation.Interfaces
{
    public interface IOriginator
    {
        // Originator kendi snapshot'ını oluşturur ve geri yükler
        IMemento Save(string label);
        void Restore(IMemento memento);
    }
}

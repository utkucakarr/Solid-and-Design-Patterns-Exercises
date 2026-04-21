namespace Memento_Implementation.Interfaces
{
    public interface IMemento
    {
        // Dışarıya yalnızca meta bilgi açık — iç state gizli kalır
        string Label { get; }
        DateTime SavedAt { get; }
    }
}

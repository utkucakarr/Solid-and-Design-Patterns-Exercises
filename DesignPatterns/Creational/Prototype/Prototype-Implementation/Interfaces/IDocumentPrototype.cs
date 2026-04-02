namespace Prototype_Implementation.Interfaces
{
    public interface IDocumentPrototype<T> where T : class
    {
        T Clone(); // Shallow kopya
        T DeepClone(); // Deep kopya
        string DocumentType { get; }
    }
}

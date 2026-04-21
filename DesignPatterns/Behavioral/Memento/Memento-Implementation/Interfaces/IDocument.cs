namespace Memento_Implementation.Interfaces
{
    public interface IDocument : IOriginator
    {
        string Title { get; }
        string Content { get; }
        IReadOnlyList<string> Tags { get; }

        void SetTitle(string title);
        void SetContent(string content);
        void AddTag(string tag);
        void RemoveTag(string tag);
    }
}

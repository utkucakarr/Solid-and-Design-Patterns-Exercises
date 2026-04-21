namespace Memento_Implementation.Models
{
    public class DocumentState
    {
        public string Title { get; init; }
        public string Content { get; init; }
        public IReadOnlyList<string> Tags { get; init; }

        public DocumentState(string title, string content, IEnumerable<string> tags)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(title, nameof(title));
            ArgumentNullException.ThrowIfNull(tags, nameof(tags));

            Title = title;
            Content = content;
            // Deep copy — dış değişiklikler snapshot'ı etkilemez
            Tags = tags.ToList().AsReadOnly();
        }
    }
}

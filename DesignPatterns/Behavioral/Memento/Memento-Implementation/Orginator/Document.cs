using Memento_Implementation.Interfaces;
using Memento_Implementation.Models;

namespace Memento_Implementation.Orginator
{
    public class Document : IDocument
    {
        private string _title;
        private string _content;
        private readonly List<string> _tags;

        public string Title => _title;
        public string Content => _content;
        public IReadOnlyList<string> Tags => _tags.AsReadOnly();

        public Document(string title)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(title, nameof(title));

            _title = title;
            _content = string.Empty;
            _tags = new List<string>();
        }

        // SetTitle — state değiştirir, snapshot almaz (Caretaker'ın sorumluluğu)
        public void SetTitle(string title)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(title, nameof(title));
            _title = title;
        }

        // SetContent — state değiştirir, snapshot almaz
        public void SetContent(string content)
        {
            ArgumentNullException.ThrowIfNull(content, nameof(content));
            _content = content;
        }

        // AddTag — state değiştirir, snapshot almaz
        public void AddTag(string tag)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(tag, nameof(tag));

            if (!_tags.Contains(tag))
                _tags.Add(tag);
        }

        // RemoveTag — state değiştirir, snapshot almaz
        public void RemoveTag(string tag)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(tag, nameof(tag));
            _tags.Remove(tag);
        }

        // Save — Originator kendi snapshot'ını oluşturur
        // DocumentState deep copy ile oluşturulur — dış değişikliklerden korunur
        public IMemento Save(string label)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(label, nameof(label));

            var state = new DocumentState(_title, _content, _tags);
            return new DocumentMemento(state, label);
        }

        // Restore — Originator kendi state'ini geri yükler
        // Memento'nun iç state'ine yalnızca Originator erişir (internal)
        public void Restore(IMemento memento)
        {
            ArgumentNullException.ThrowIfNull(memento, nameof(memento));

            if (memento is not DocumentMemento documentMemento)
                throw new InvalidOperationException(
                    $"Geçersiz memento tipi: {memento.GetType().Name}");

            // Deep copy ile geri yükleme — snapshot bozulmaz
            _title = documentMemento.State.Title;
            _content = documentMemento.State.Content;
            _tags.Clear();
            _tags.AddRange(documentMemento.State.Tags);
        }
    }
}

using Memento_Implementation.Interfaces;

namespace Memento_Implementation.Models
{
    public class DocumentMemento : IMemento
    {
        // State dışarıya kapalı — sadece Originator erişebilir
        // internal keyword ile assembly dışından erişim engellendi
        internal DocumentState State { get; }

        public string Label { get; }
        public DateTime SavedAt { get; }

        internal DocumentMemento(DocumentState state, string label)
        {
            ArgumentNullException.ThrowIfNull(state, nameof(state));
            ArgumentException.ThrowIfNullOrWhiteSpace(label, nameof(label));

            State = state;
            Label = label;
            SavedAt = DateTime.UtcNow;
        }
    }
}

using Memento_Implementation.Interfaces;

namespace Memento_Implementation.Caretaker
{
    public class DocumentHistory : IDocumentHistory
    {
        private readonly Stack<IMemento> _undoStack = new();
        private readonly Stack<IMemento> _redoStack = new();

        // Caretaker — undo/redo yığınlarını yönetir
        // Document'ın iç state'ini bilmez — sadece IMemento arayüzünü bilir

        public bool CanUndo => _undoStack.Count > 0;
        public bool CanRedo => _redoStack.Count > 0;
        public int UndoCount => _undoStack.Count;
        public int RedoCount => _redoStack.Count;

        // Yeni snapshot eklendiğinde Redo stack temizlenir
        // Yeni değişiklik → redo geçmişi geçersiz olur
        public void Push(IMemento memento)
        {
            ArgumentNullException.ThrowIfNull(memento, nameof(memento));

            _undoStack.Push(memento);
            _redoStack.Clear();
        }

        // Undo — en son snapshot'ı undo stack'ten çıkarır
        public IMemento? PopUndo()
        {
            return _undoStack.Count > 0 ? _undoStack.Pop() : null;
        }

        // Redo — en son geri alınan snapshot'ı redo stack'ten çıkarır
        public IMemento? PopRedo()
        {
            return _redoStack.Count > 0 ? _redoStack.Pop() : null;
        }

        // Redo stack'i temizle — yeni değişiklik yapıldığında çağrılır
        public void ClearRedo()
        {
            _redoStack.Clear();
        }

        // Undo için mevcut state'i redo stack'e taşır
        public void MoveToRedo(IMemento memento)
        {
            ArgumentNullException.ThrowIfNull(memento, nameof(memento));
            _redoStack.Push(memento);
        }

        // Redo için mevcut state'i undo stack'e taşır
        public void MoveToUndo(IMemento memento)
        {
            ArgumentNullException.ThrowIfNull(memento, nameof(memento));
            _undoStack.Push(memento);
        }
    }
}

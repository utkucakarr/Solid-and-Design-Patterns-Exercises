namespace Memento_Implementation.Interfaces
{
    public interface IDocumentHistory
    {
        // Caretaker — undo/redo yığınlarını yönetir
        bool CanUndo { get; }
        bool CanRedo { get; }
        int UndoCount { get; }
        int RedoCount { get; }

        void Push(IMemento memento);
        IMemento? PopUndo();
        IMemento? PopRedo();
        void ClearRedo();
    }
}

using Memento_Implementation.Caretaker;
using Memento_Implementation.Interfaces;
using Memento_Implementation.Models;

namespace Memento_Implementation.Orginator
{
    public class DocumentEditor : IDocumentEditor
    {
        private readonly IDocument _document;
        private readonly DocumentHistory _history;

        public DocumentEditor(IDocument document, DocumentHistory history)
        {
            ArgumentNullException.ThrowIfNull(document, nameof(document));
            ArgumentNullException.ThrowIfNull(history, nameof(history));

            _document = document;
            _history = history;
        }

        // SetTitle — önce snapshot alınır, sonra değişiklik yapılır
        public DocumentResult SetTitle(string title)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(title, nameof(title));

            // Değişiklik öncesi mevcut state snapshot'a alınır
            _history.Push(_document.Save($"Başlık değişti: '{_document.Title}' → '{title}'"));

            _document.SetTitle(title);

            return BuildSuccess($"Başlık '{title}' olarak güncellendi.");
        }

        // SetContent — önce snapshot alınır, sonra değişiklik yapılır
        public DocumentResult SetContent(string content)
        {
            ArgumentNullException.ThrowIfNull(content, nameof(content));

            var preview = content.Length > 30
                ? content[..30] + "..."
                : content;

            // Değişiklik öncesi mevcut state snapshot'a alınır
            _history.Push(_document.Save($"İçerik güncellendi: '{preview}'"));

            _document.SetContent(content);

            return BuildSuccess($"İçerik güncellendi.");
        }

        // AddTag — önce snapshot alınır, sonra tag eklenir
        public DocumentResult AddTag(string tag)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(tag, nameof(tag));

            // Değişiklik öncesi mevcut state snapshot'a alınır
            _history.Push(_document.Save($"Etiket eklendi: '{tag}'"));

            _document.AddTag(tag);

            return BuildSuccess($"'{tag}' etiketi eklendi.");
        }

        // RemoveTag — önce snapshot alınır, sonra tag çıkarılır
        public DocumentResult RemoveTag(string tag)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(tag, nameof(tag));

            // Değişiklik öncesi mevcut state snapshot'a alınır
            _history.Push(_document.Save($"Etiket silindi: '{tag}'"));

            _document.RemoveTag(tag);

            return BuildSuccess($"'{tag}' etiketi silindi.");
        }

        // Undo — mevcut state redo'ya taşınır, son snapshot geri yüklenir
        public DocumentResult Undo()
        {
            if (!_history.CanUndo)
                return DocumentResult.Fail("Geri alınacak işlem bulunmuyor.");

            // Mevcut state redo stack'e kaydedilir
            var currentSnapshot = _document.Save("Undo öncesi state");
            _history.MoveToRedo(currentSnapshot);

            // Son snapshot undo stack'ten alınır ve geri yüklenir
            var memento = _history.PopUndo();
            _document.Restore(memento!);

            return BuildSuccess($"Geri alındı -> '{memento!.Label}'");
        }

        // Redo — mevcut state undo'ya taşınır, redo snapshot geri yüklenir
        public DocumentResult Redo()
        {
            if (!_history.CanRedo)
                return DocumentResult.Fail("İleri alınacak işlem bulunmuyor.");

            // Mevcut state undo stack'e kaydedilir
            var currentSnapshot = _document.Save("Redo öncesi state");
            _history.MoveToUndo(currentSnapshot);

            // Son redo snapshot alınır ve geri yüklenir
            var memento = _history.PopRedo();
            _document.Restore(memento!);

            return BuildSuccess($"İleri alındı -> '{memento!.Label}'");
        }

        // Mevcut state'i döner — snapshot almaz
        public DocumentResult GetCurrentState()
        {
            return BuildSuccess("Mevcut durum.");
        }

        // Her metot aynı result formatını kullanır — DRY
        private DocumentResult BuildSuccess(string message) =>
            DocumentResult.Success(
                message: message,
                title: _document.Title,
                content: _document.Content,
                tags: _document.Tags,
                canUndo: _history.CanUndo,
                canRedo: _history.CanRedo
            );
    }
}

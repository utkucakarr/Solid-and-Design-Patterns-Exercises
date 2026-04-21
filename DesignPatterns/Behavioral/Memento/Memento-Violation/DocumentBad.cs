namespace Memento_Violation
{
    public class DocumentBad
    {
        public string Title { get; private set; }
        public string Content { get; private set; }
        public List<string> Tags { get; private set; }

        // Geçmiş yığını doğrudan Document içinde tutuluyor — SRP ihlali
        // Document hem içeriği yönetiyor hem de geçmişini yönetiyor
        private readonly Stack<DocumentSnapshot> _undoStack = new();
        private readonly Stack<DocumentSnapshot> _redoStack = new();

        public DocumentBad(string title)
        {
            // Guard clause yok — null veya boş başlık kabul ediliyor
            Title = title;
            Content = string.Empty;
            Tags = new List<string>();
        }

        public void SetTitle(string title)
        {
            // Her değişiklik öncesi snapshot alınıyor ama Redo stack temizlenmiyor
            _undoStack.Push(new DocumentSnapshot(Title, Content, Tags));
            Title = title;
            // Redo stack'i burada temizlemeyi unuttuk — tutarsız state
        }

        public void SetContent(string content)
        {
            // Snapshot alınıyor ama Tags referans kopyası — deep copy yok
            _undoStack.Push(new DocumentSnapshot(Title, Content, Tags));
            Content = content;
            // Redo stack temizlenmiyor — yeni değişiklik sonrası redo tutarsız
        }

        public void AddTag(string tag)
        {
            // Tag listesi referans olarak saklandığı için
            // snapshot sonrası yapılan tag değişikliği geçmişi de etkiliyor
            _undoStack.Push(new DocumentSnapshot(Title, Content, Tags));
            Tags.Add(tag);
        }

        public void Undo()
        {
            if (_undoStack.Count == 0)
            {
                Console.WriteLine("Geri alınacak işlem yok.");
                return;
            }

            // Redo için mevcut state kaydediliyor ama deep copy yok
            _redoStack.Push(new DocumentSnapshot(Title, Content, Tags));

            var snapshot = _undoStack.Pop();
            Title = snapshot.Title;
            Content = snapshot.Content;
            // Tags referans atması — snapshot'taki liste doğrudan atanıyor
            Tags = snapshot.Tags;
        }

        public void Redo()
        {
            if (_redoStack.Count == 0)
            {
                Console.WriteLine("İleri alınacak işlem yok.");
                return;
            }

            // Undo için mevcut state kaydediliyor ama deep copy yok
            _undoStack.Push(new DocumentSnapshot(Title, Content, Tags));

            var snapshot = _redoStack.Pop();
            Title = snapshot.Title;
            Content = snapshot.Content;
            // Tags referans atması — aynı sorun burada da devam ediyor
            Tags = snapshot.Tags;
        }

        // Snapshot'ın iç yapısı dışarıdan erişilebilir
        // Caretaker rolü Document'a gömülmüş — test etmek çok zor
        public IReadOnlyList<DocumentSnapshot> GetUndoHistory() =>
            _undoStack.ToList().AsReadOnly();

        public void PrintState()
        {
            Console.WriteLine($" Başlık  : {Title}");
            Console.WriteLine($" İçerik  : {Content}");
            Console.WriteLine($" Etiketler: [{string.Join(", ", Tags)}]");
            Console.WriteLine($" Undo    : {_undoStack.Count} adım");
            Console.WriteLine($" Redo    : {_redoStack.Count} adım");
        }
    }
}
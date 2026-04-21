namespace Memento_Violation
{
    public class DocumentSnapshot
    {
        // Snapshot nesnesi tamamen public — dışarıdan her alan değiştirilebilir
        public string Title { get; set; }
        public string Content { get; set; }
        public List<string> Tags { get; set; }
        public DateTime SavedAt { get; set; }

        public DocumentSnapshot(string title, string content, List<string> tags)
        {
            Title = title;
            Content = content;
            Tags = tags;
            SavedAt = DateTime.Now;
        }
    }
}

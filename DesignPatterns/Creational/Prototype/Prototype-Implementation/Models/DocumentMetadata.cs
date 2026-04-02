namespace Prototype_Implementation.Models
{
    public class DocumentMetadata
    {
        public string Author { get; set; }
        public string Department { get; set; }
        public string Version { get; set; }
        public DateTime CreatedAt { get; set; }

        public DocumentMetadata(string author, string department, string version)
        {
            Author = author;
            Department = department;
            Version = version;
            CreatedAt = DateTime.Now;
        }

        // Deep Copy için
        public DocumentMetadata Clone()
            => new DocumentMetadata(Author, Department, Version)
            {
                CreatedAt = this.CreatedAt
            };

        public override string ToString()
             => $"Yazar: {Author} | Departman: {Department} | Versiyon: {Version}";
    }
}

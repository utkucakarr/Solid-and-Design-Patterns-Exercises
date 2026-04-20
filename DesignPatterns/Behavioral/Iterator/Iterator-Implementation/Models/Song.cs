namespace Iterator_Implementation.Models
{
    public sealed class Song
    {
        public string Title { get; }
        public string Artist { get; }
        public int DurationSeconds { get; }
        public bool IsFavorite { get; }

        public Song(string title, string artist, int durationSeconds, bool isFavorite = false)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(title, nameof(title));
            ArgumentException.ThrowIfNullOrWhiteSpace(artist, nameof(artist));
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(durationSeconds, nameof(durationSeconds));

            Title = title;
            Artist = artist;
            DurationSeconds = durationSeconds;
            IsFavorite = isFavorite;
        }

        public override string ToString() => $"{Artist} - {Title} ({DurationSeconds}s)";
    }
}

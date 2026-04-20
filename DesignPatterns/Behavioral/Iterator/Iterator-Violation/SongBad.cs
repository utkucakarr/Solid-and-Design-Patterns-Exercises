namespace Iterator_Violation
{
    public class SongBad
    {
        public string Title { get; set; }
        public string Artist { get; set; }
        public int DurationSeconds { get; set; }
        public bool IsFavorite { get; set; }

        public SongBad(string title, string artist, int durationSeconds, bool isFavorite)
        {
            // Guard clause yok — null, boş string veya negatif süre kabul ediliyor
            Title = title;
            Artist = artist;
            DurationSeconds = durationSeconds;
            IsFavorite = isFavorite;
        }
    }
}

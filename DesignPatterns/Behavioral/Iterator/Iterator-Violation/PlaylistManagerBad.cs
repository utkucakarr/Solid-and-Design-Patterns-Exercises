namespace Iterator_Violation
{
    // Koleksiyonun iç yapısını doğrudan dışarıya açan yönetici
    public class PlaylistManagerBad
    {
        // List<Song_Bad> public — herkes ekleyebilir, silebilir, sıralayabilir; kapsülleme yok
        public List<SongBad> Songs { get; } = new();

        public void AddSong(SongBad song)
        {
            // Null kontrolü yok
            Songs.Add(song);
        }

        // Her traversal stratejisi için ayrı metot — istemci neyi ne zaman çağıracağını bilmek zorunda
        public List<SongBad> GetAllSongs() => Songs;

        // Koleksiyon tipi (List) değişirse bu metot da kırılır
        public List<SongBad> GetShuffledSongs()
        {
            var copy = new List<SongBad>(Songs);
            var rng = new Random(42); // Sabit seed — her çağrıda aynı "karışıklık"
            for (int i = copy.Count - 1; i > 0; i--)
            {
                int j = rng.Next(i + 1);
                (copy[i], copy[j]) = (copy[j], copy[i]);
            }
            return copy;
        }

        // Filtreleme mantığı koleksiyonla iç içe geçmiş; istemci kendi filtresini de yazabilir — tutarsızlık
        public List<SongBad> GetFavoriteSongs() =>
            Songs.Where(s => s.IsFavorite).ToList();

        // İndeks bazlı erişim — sınır kontrolü yok, IndexOutOfRangeException riski
        public SongBad GetSongAt(int index) => Songs[index];
    }
}

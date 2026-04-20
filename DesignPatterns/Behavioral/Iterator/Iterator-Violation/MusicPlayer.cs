namespace Iterator_Violation
{
    public class MusicPlayer
    {
        // PlaylistManager_Bad'in iç List yapısına doğrudan bağımlı
        public void PlayAll(PlaylistManagerBad manager)
        {
            // İstemci indeks bilmek ve Songs property'sine erişmek zorunda
            for (int i = 0; i < manager.Songs.Count; i++)
            {
                var song = manager.Songs[i]; // Songs public olmak zorunda
                Console.WriteLine($" [{i + 1}] {song.Artist} - {song.Title} ({song.DurationSeconds}s)");
            }
        }

        // Strateji değişince bu metot da değişmeli — OCP ihlali
        public void PlayShuffled(PlaylistManagerBad manager)
        {
            var shuffled = manager.GetShuffledSongs();
            foreach (var song in shuffled)
                Console.WriteLine($" {song.Artist} - {song.Title} ({song.DurationSeconds}s)");
        }

        // Yeni strateji(ör.alfabetik, son eklenen) eklemek için hem Manager hem Player değişmeli
    public void PlayFavorites(PlaylistManagerBad manager)
        {
            var favorites = manager.GetFavoriteSongs();
            if (!favorites.Any()) // İstemci boş kontrol yapmak zorunda
            {
                Console.WriteLine(" Favori şarkı bulunamadı.");
                return;
            }
            foreach (var song in favorites)
                Console.WriteLine($" {song.Artist} - {song.Title} ({song.DurationSeconds}s)");
        }
    }
}

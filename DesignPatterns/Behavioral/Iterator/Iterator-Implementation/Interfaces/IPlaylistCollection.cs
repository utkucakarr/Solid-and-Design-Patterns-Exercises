using Iterator_Implementation.Models;

namespace Iterator_Implementation.Interfaces
{
    // Aggregate arayüzü — farklı iterator stratejilerini fabrika metotlarla üretir
    public interface IPlaylistCollection
    {
        int Count { get; }
        void AddSong(Song song);
        IIterator<Song> CreateSqeuentialIterator(); // Sıralı gezinim
        IIterator<Song> CreateShuffleIterator(); // Karışık gezinim
        IIterator<Song> CreateFavoriteIterator(); // Filtreli gezinim
    }
}

using Iterator_Implementation.Interfaces;
using Iterator_Implementation.Iterators;
using Iterator_Implementation.Models;

namespace Iterator_Implementation.Collections
{
    // Aggregate — iç yapıyı gizler, iterator üretir
    public class Playlist : IPlaylistCollection
    {
        private readonly List<Song> _songs = new();

        public string Name { get; }
        public int Count => _songs.Count;

        public Playlist(string name)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name, nameof(name));
            Name = name;
        }

        public void AddSong(Song song)
        {
            ArgumentNullException.ThrowIfNull(song, nameof(song));
            _songs.Add(song); // _songs hiçbir zaman dışarıya açılmıyor
        }

        // Her çağrıda bağımsız yeni iterator — birden fazla eş zamanlı gezinim desteklenir
        public IIterator<Song> CreateSqeuentialIterator() => new SequentialIterator(_songs);
        public IIterator<Song> CreateShuffleIterator() => new ShuffleIterator(_songs);
        public IIterator<Song> CreateFavoriteIterator() => new FavoriteIterator(_songs);
    }
}

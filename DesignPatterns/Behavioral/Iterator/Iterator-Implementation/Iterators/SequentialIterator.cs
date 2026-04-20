using Iterator_Implementation.Interfaces;
using Iterator_Implementation.Models;

namespace Iterator_Implementation.Iterators
{
    // Sıralı gezinim - şarkıları eklenme sırasıyla iter
    public sealed class SequentialIterator : IIterator<Song>
    {
        private readonly IReadOnlyList<Song> _songs;
        private int _index;

        public SequentialIterator(IReadOnlyList<Song> songs)
        {
            ArgumentNullException.ThrowIfNull(songs, nameof(songs));
            _songs = songs;
            _index = 0;
        }

        public bool HasNext() => _index < _songs.Count; // İstemci iç yapıyı bilmeden kontrol eder

        public Song Next()
        {
            if (!HasNext())
                throw new InvalidOperationException("Iterator sonuna ulaşıldı. Devam için Reset() çağırın.");
            return _songs[_index++]; // Sıralı erişim, iç yapı tamamen gizli
        }

        public void Reset() => _index = 0; // Başa dön
    }
}

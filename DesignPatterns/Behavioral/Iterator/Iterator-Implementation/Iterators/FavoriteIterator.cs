using Iterator_Implementation.Interfaces;
using Iterator_Implementation.Models;

namespace Iterator_Implementation.Iterators
{
    // Filtreli gezinim — yalnızca favori şarkıları iter
    public class FavoriteIterator : IIterator<Song>
    {
        private readonly IReadOnlyList<Song> _favorites;
        private int _index;

        public FavoriteIterator(IReadOnlyList<Song> songs)
        {
            ArgumentNullException.ThrowIfNull(songs, nameof(songs));
            _favorites = songs.Where(s => s.IsFavorite).ToList(); // Filtreleme iterator içinde kapsüllendi
            _index = 0;
        }

        public bool HasNext() => _index < _favorites.Count;

        public Song Next()
        {
            if (!HasNext())
                throw new InvalidOperationException("Iterator sonuna ulaşıldı. Devam için Reset() çağırın.");
            return _favorites[_index++]; // Sadece favoriler döner — istemci filtreyi bilmiyor
        }

        public void Reset() => _index = 0;
    }
}
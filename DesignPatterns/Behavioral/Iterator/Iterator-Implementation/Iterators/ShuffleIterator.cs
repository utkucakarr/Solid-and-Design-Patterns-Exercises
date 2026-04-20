using Iterator_Implementation.Interfaces;
using Iterator_Implementation.Models;

namespace Iterator_Implementation.Iterators
{
    // Karışık gezinim — Fisher-Yates algoritması ile shuffle
    public class ShuffleIterator : IIterator<Song>
    {
        private readonly List<Song> _shuffled;
        private int _index;

        public ShuffleIterator(IReadOnlyList<Song> songs, int? seed = null)
        {
            ArgumentNullException.ThrowIfNull(songs, nameof(songs));
            _shuffled = new List<Song>(songs);
            _index = 0;
            Shuffle(seed); // Constructor'da bir kez karıştır
        }

        private void Shuffle(int? seed)
        {
            var rng = seed.HasValue ? new Random(seed.Value) : new Random();
            for (int i = _shuffled.Count - 1; i > 0; i--)
            {
                int j = rng.Next(i + 1);
                (_shuffled[i], _shuffled[j]) = (_shuffled[j], _shuffled[i]); // Fisher-Yates swap
            }
        }

        public bool HasNext() => _index < _shuffled.Count;

        public Song Next()
        {
            if (!HasNext())
                throw new InvalidOperationException("Iterator sonuna ulaşıldı. Devam için Reset() çağırın.");
            return _shuffled[_index++]; // Karışık sırada erişim
        }

        public void Reset() => _index = 0; // Aynı karışık sırayla başa döner
    }
}
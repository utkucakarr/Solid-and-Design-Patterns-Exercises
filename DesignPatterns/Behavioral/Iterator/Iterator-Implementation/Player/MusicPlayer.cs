using Iterator_Implementation.Interfaces;
using Iterator_Implementation.Models;

namespace Iterator_Implementation.Player
{
    // Iterator'ı tüketen istemci — koleksiyonun iç yapısından tamamen bağımsız
    public class MusicPlayer : IMusicPlayer
    {
        public PlaylistResult Play(IIterator<Song> iterator)
        {
            ArgumentNullException.ThrowIfNull(iterator, nameof(iterator));

            if (!iterator.HasNext())
                return PlaylistResult.Fail("Çalınacak şarkı bulunamadı."); // Boş koleksiyon güvenli yönetim

            var played = new List<string>();
            int totalDuration = 0;

            while (iterator.HasNext()) // HasNext/Next ile gezinim — List mi, Array mi bilmez
            {
                var song = iterator.Next();
                played.Add($"{song.Artist} - {song.Title}");
                totalDuration += song.DurationSeconds;
                Console.WriteLine($"{song}");
            }

            return PlaylistResult.Success(played.AsReadOnly(), totalDuration); // Immutable result
        }
    }
}

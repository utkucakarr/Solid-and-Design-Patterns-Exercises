using Iterator_Implementation.Models;

namespace Iterator_Implementation.Interfaces
{
    // MusicPlayer sözleşmesi — DI ve test edilebilirlik için
    public interface IMusicPlayer
    {
        PlaylistResult Play(IIterator<Song> iterator);
    }
}

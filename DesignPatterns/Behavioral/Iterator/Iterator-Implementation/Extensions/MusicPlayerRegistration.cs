using Iterator_Implementation.Collections;
using Iterator_Implementation.Interfaces;
using Iterator_Implementation.Models;
using Iterator_Implementation.Player;
using Microsoft.Extensions.DependencyInjection;

namespace Iterator_Implementation.Extensions
{
    public static class MusicPlayerRegistration
    {
        public static IServiceCollection AddMusicPlayer(this IServiceCollection services)
        {
            services.AddScoped<IPlaylistCollection>(_ =>
            {
                var playlist = new Playlist("Rock Klasikleri");
                playlist.AddSong(new Song("Bohemian Rhapsody", "Queen", 354, isFavorite: true));
                playlist.AddSong(new Song("Stairway to Heaven", "Led Zeppelin", 482));
                playlist.AddSong(new Song("Hotel California", "Eagles", 391, isFavorite: true));
                playlist.AddSong(new Song("Imagine", "John Lennon", 187));
                playlist.AddSong(new Song("Smells Like Teen Spirit", "Nirvana", 301));
                return playlist;
            });

            services.AddScoped<IMusicPlayer, MusicPlayer>();

            return services;
        }
    }
}

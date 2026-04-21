using Iterator_Violation;
using Iterator_Implementation.Collections;
using Iterator_Implementation.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Iterator_Implementation.Extensions;

Console.OutputEncoding = System.Text.Encoding.UTF8;

Console.WriteLine("╔═══════════════════════════════════════╗");
Console.WriteLine("║          İHLAL YAKLAŞIMI           ║");
Console.WriteLine("╚═══════════════════════════════════════╝\n");

var manager = new PlaylistManagerBad();
manager.AddSong(new SongBad("Bohemian Rhapsody", "Queen", 354, isFavorite: true));
manager.AddSong(new SongBad("Stairway to Heaven", "Led Zeppelin", 482, isFavorite: false));
manager.AddSong(new SongBad("Hotel California", "Eagles", 391, isFavorite: true));
manager.AddSong(new SongBad("Imagine", "John Lennon", 187, isFavorite: false));

var badPlayer = new Iterator_Violation.MusicPlayer();

Console.WriteLine(" Sorun 1: Songs listesi public — herkes okuyabilir, yazabilir, silebilir:");
Console.WriteLine($" manager.Songs.Count    = {manager.Songs.Count}");
Console.WriteLine($" manager.Songs[0].Title = \"{manager.Songs[0].Title}\" (doğrudan erişim!)\n");

Console.WriteLine(" Sıralı çalma (for döngüsü + indeks erişimi):");
badPlayer.PlayAll(manager);

Console.WriteLine("\n Sorun 2: Strateji değiştirmek için farklı metot çağrısı gerekiyor:");
Console.WriteLine(" Karışık çalma:");
badPlayer.PlayShuffled(manager);

Console.WriteLine("\n Sorun 3: Yeni strateji eklemek için PlaylistManager_Bad VE MusicPlayer_Bad değişmeli (OCP ihlali):");
Console.WriteLine(" Favori çalma:");
badPlayer.PlayFavorites(manager);

Console.WriteLine();


Console.WriteLine("╔═══════════════════════════════════════╗");
Console.WriteLine("║        DOĞRU YAKLAŞIM (DI)         ║");
Console.WriteLine("╚═══════════════════════════════════════╝\n");

var services = new ServiceCollection();

services.AddMusicPlayer();

using var provider = services.BuildServiceProvider();
var collection = provider.GetRequiredService<IPlaylistCollection>();
var player = provider.GetRequiredService<IMusicPlayer>();

// Sıralı çalma
Console.WriteLine(" Sıralı çalma — SequentialIterator:");
var seqResult = player.Play(collection.CreateSqeuentialIterator());
Console.WriteLine($" {seqResult.Message} | Süre: {seqResult.TotalDurationSeconds}s | Kod: {seqResult.TracingCode}\n");

// Karışık çalma — Aynı MusicPlayer, sadece farklı Iterator
Console.WriteLine(" Karışık çalma — ShuffleIterator (aynı MusicPlayer, sadece farklı iterator!):");
var shuffleResult = player.Play(collection.CreateShuffleIterator());
Console.WriteLine($" {shuffleResult.Message} | Süre: {shuffleResult.TotalDurationSeconds}s | Kod: {shuffleResult.TracingCode}\n");

// Favori çalma — Filtreleme tamamen iterator içinde, istemci bilmiyor
Console.WriteLine(" Favori çalma — FavoriteIterator (filtreleme istemciden gizli):");
var favoriteResult = player.Play(collection.CreateFavoriteIterator());
Console.WriteLine($" {favoriteResult.Message} | Süre: {favoriteResult.TotalDurationSeconds}s | Kod: {favoriteResult.TracingCode}\n");

// Boş playlist — güvenli hata yönetimi
Console.WriteLine(" Boş playlist — PlaylistResult.Fail ile güvenli yönetim:");
var emptyPlaylist = new Playlist("Boş Liste");
var emptyResult = player.Play(emptyPlaylist.CreateFavoriteIterator());
Console.WriteLine($" {emptyResult.Message} | Başarı: {emptyResult.IsSuccess}\n");

Console.WriteLine(" Yeni strateji eklemek için sadece yeni IIterator<Song> implement edilir.");
Console.WriteLine(" MusicPlayer, Playlist veya mevcut iterator'lar DEĞİŞMEZ — OCP sağlandı.");
# Iterator Pattern — Playlist Yönetim Sistemi

> *"Bir koleksiyonun elemanlarına, koleksiyonun iç yapısını açığa çıkarmadan sırayla erişmek için bir yol sağlar."*
> — GoF (Gang of Four)

---

## Pattern Nedir?

**Iterator Pattern**, bir koleksiyonun (liste, ağaç, stack vb.) elemanlarını gezmek için standart bir arayüz tanımlar. İstemci, koleksiyonun nasıl saklandığını (List mi? Array mi? LinkedList mi?) **hiç bilmeden** tüm elemanlara ulaşabilir.

### Ne Zaman Kullanılır?

- Koleksiyonun iç yapısını istemciden **gizlemek** istediğinde
- Aynı koleksiyon üzerinde **farklı gezinim stratejileri** (sıralı, karışık, filtreli) gerektiğinde
- Birden fazla **eş zamanlı iterator** çalıştırmak istediğinde
- Koleksiyon tipi değişse bile istemci kodunun **değişmemesi** gerektiğinde

### Gerçek Hayat Örnekleri

| Ortam | Iterator Kullanımı |
|---|---|
| Spotify / Apple Music | Sequential, Shuffle, Repeat modu |
| .NET `IEnumerable<T>` | `foreach` döngüsünün altındaki her koleksiyon |
| Veritabanı Cursor | Satır satır okuma, bellek dostu |
| Dosya Sistemi | Klasör içeriğini recursive gezme |
| Git Log | Commit geçmişinde ileri/geri gezinim |

---

## Senaryo

> "Bir müzik streaming uygulaması geliştiriyoruz. Kullanıcının playlist'i üzerinde
> üç farklı gezinim modu var: sıralı çalma, karışık çalma ve yalnızca favorileri çalma.
> Koleksiyonun iç yapısını (List, Array vs.) istemci koduna açmadan bu üç stratejiyi
> nasıl tasarlarız?"

---

## Kötü Kullanım

```csharp
// Songs listesi tamamen public — herkes erişebilir, değiştirebilir
public class PlaylistManager_Bad
{
    public List<Song_Bad> Songs { get; } = new(); // Kapsülleme yok

    // Her strateji için ayrı metot — istemci hangisini ne zaman çağıracak?
    public List<Song_Bad> GetAllSongs()     => Songs;
    public List<Song_Bad> GetShuffledSongs() { ... }
    public List<Song_Bad> GetFavoriteSongs() { ... }
}

// MusicPlayer, Songs.Count ve Songs[i] bilmek zorunda
public class MusicPlayer_Bad
{
    public void PlayAll(PlaylistManager_Bad manager)
    {
        for (int i = 0; i < manager.Songs.Count; i++) // İç yapıya bağımlı
        {
            var song = manager.Songs[i]; // List tipi değişirse bu kırılır
            Console.WriteLine($"▶ {song.Title}");
        }
    }

    // Yeni strateji eklemek için hem Manager hem Player değişmeli — OCP ihlali
    public void PlayShuffled(PlaylistManager_Bad manager) { ... }
    public void PlayFavorites(PlaylistManager_Bad manager) { ... }
}
```

### Sonuçlar

| Sorun | Açıklama |
|---|---|
| Kapsülleme yok | `Songs` listesi public — dış kod ekleyebilir, silebilir |
| İç yapıya bağımlı | `List<T>` yerine başka yapı gelse istemci kırılır |
| OCP ihlali | Yeni strateji = hem Manager hem Player değişimi |
| Çoklu iterator yok | Eş zamanlı iki farklı gezinim desteklenmiyor |
| Test edilemez | Koleksiyon mock'lanamaz, iç yapı sızıyor |

---

## Doğru Kullanım

```csharp
// GoF Iterator arayüzü
public interface IIterator<T>
{
    bool HasNext();
    T    Next();
    void Reset();
}

// Aggregate — iç listeyi tamamen gizler
public sealed class Playlist : IPlaylistCollection
{
    private readonly List<Song> _songs = new(); // Dışarıya hiç açılmıyor

    public IIterator<Song> CreateSequentialIterator() => new SequentialIterator(_songs);
    public IIterator<Song> CreateShuffleIterator()    => new ShuffleIterator(_songs);
    public IIterator<Song> CreateFavoriteIterator()   => new FavoriteIterator(_songs);
}

// İstemci: IIterator<Song> dışında hiçbir şey bilmiyor
public sealed class MusicPlayer : IMusicPlayer
{
    public PlaylistResult Play(IIterator<Song> iterator)
    {
        ArgumentNullException.ThrowIfNull(iterator, nameof(iterator));

        if (!iterator.HasNext())
            return PlaylistResult.Fail("Çalınacak şarkı bulunamadı.");

        var played = new List<string>();
        int total  = 0;

        while (iterator.HasNext())
        {
            var song = iterator.Next(); // ✅ List mi? Array mi? Bilmez, bilmek zorunda değil
            played.Add($"{song.Artist} - {song.Title}");
            total += song.DurationSeconds;
        }

        return PlaylistResult.Success(played.AsReadOnly(), total);
    }
}

// Strateji değişimi = sadece farklı iterator, MusicPlayer dokunulmaz
player.Play(playlist.CreateSequentialIterator()); // Sıralı
player.Play(playlist.CreateShuffleIterator());    // Karışık
player.Play(playlist.CreateFavoriteIterator());   // Sadece favoriler
```

---

## Pattern'in Yaptığı İşlem

| Adım | Eylem | Sorumlu Sınıf |
|------|-------|---------------|
| 1 | Şarkı ekleme | `Playlist.AddSong()` |
| 2 | Iterator fabrikası çağrısı | `Playlist.CreateXxxIterator()` |
| 3 | Shuffle/filtreleme kapsülleme | `ShuffleIterator` / `FavoriteIterator` |
| 4 | `HasNext()` ile güvenli kontrol | `IIterator<T>` |
| 5 | `Next()` ile eleman alma | `IIterator<T>` |
| 6 | `PlaylistResult` üretimi | `MusicPlayer.Play()` |
| 7 | Reset ile başa dönme | `IIterator<T>.Reset()` |

---

## Farkın Özeti

| Özellik | Bad (PlaylistManager_Bad) | Good (Playlist + Iterator) |
|---|---|---|
| İç yapı | `Songs` listesi public | `_songs` tamamen gizli |
| Gezinim | `for` + indeks erişimi | `HasNext()` / `Next()` |
| Strateji ekleme | Manager + Player değişmeli | Sadece yeni `IIterator<T>` |
| Eş zamanlı gezinim | Yok | Her `CreateXxx()` bağımsız iterator üretir |
| Koleksiyon bağımlılığı | `List<T>` hard-coded | Sıfır bağımlılık |
| Test edilebilirlik | Kötü (iç yapı sızıyor) | `IIterator<Song>` mock'lanabilir |
| Null güvenliği | Yok | Guard clause + `PlaylistResult.Fail` |

---

## Testler

### Neden Moq?

`MusicPlayer`, `IIterator<Song>` arayüzüne bağımlıdır. Moq ile gerçek `Playlist` ve `Song` listesi olmadan, sadece `HasNext` / `Next` davranışını simüle ederek player'ı izole test edebiliriz.

```csharp
var mockIterator = new Mock<IIterator<Song>>();
mockIterator.SetupSequence(i => i.HasNext())
    .Returns(true)
    .Returns(false);
mockIterator.Setup(i => i.Next()).Returns(song);

mockIterator.Verify(i => i.Next(), Times.Exactly(3));
```

### Kapsanan Senaryolar

| Kategori | Test Sayısı | Örnek |
|---|---|---|
| Happy path | 7 | Sıralı, karışık, favori gezinim |
| Moq çağrı doğrulama | 3 | `Next()` kaç kez çağrıldı? |
| Başarısız senaryo | 2 | Tükenmiş iterator'dan `Next()` |
| Koleksiyon | 2 | `AddSong`, `CreateSequentialIterator` |
| Guard clause (Theory) | 12 | Null/boş title, artist, duration ≤ 0 |
| Constructor null guard | 4 | Iterator'lar ve MusicPlayer |
| Result factory | 3 | `Success`, `Fail`, `Fail` boş string |
| **Toplam** | **34** | |


## SOLID ile Bağlantısı

| Prensip | Nasıl Sağlandı? |
|---|---|
| **S** — Single Responsibility | `Playlist` koleksiyonu yönetir; `MusicPlayer` çalar; `ShuffleIterator` karıştırır |
| **O** — Open/Closed | Yeni iterator eklemek mevcut kodu **değiştirmez**, sadece `IIterator<T>` implement edilir |
| **L** — Liskov Substitution | `SequentialIterator`, `ShuffleIterator`, `FavoriteIterator` birbirinin yerine geçebilir |
| **I** — Interface Segregation | `IIterator<T>` sadece 3 metot; `IPlaylistCollection` sadece koleksiyon sözleşmesi |
| **D** — Dependency Inversion | `MusicPlayer`, `IIterator<Song>` arayüzüne bağlı — concrete sınıfa değil |

---

## Diğer Pattern'lerle İlişkisi

| Pattern | İlişki |
|---|---|
| **Composite** | Ağaç yapılı koleksiyonları (playlist içinde playlist) gezmek için Iterator + Composite birlikte kullanılır |
| **Factory Method** | `Playlist.CreateShuffleIterator()` bir factory method'dur — iterator üretimini soyutlar |
| **Strategy** | Shuffle/Sequential/Favorite farklı **gezinim stratejileri**dir; Iterator bu stratejileri kapsüller. Strategy davranışı dışa taşır, Iterator koleksiyon içinde tutar |
| **Memento** | Iterator pozisyonu kaydedip geri yüklemek için Memento kullanılabilir |

---

## Kullanılan Teknolojiler

| Teknoloji | Versiyon | Kullanım Amacı |
|---|---|---|
| C# / .NET | 8.0 | Dil ve platform |
| Microsoft.Extensions.DependencyInjection | 10.0.6 | DI konteyneri |
| xUnit | 2.5.3 | Test framework |
| FluentAssertions | 8.9.0 | Okunabilir assertion |
| Moq | 4.20.72 | `IIterator<Song>` mock'lama |

---

## Design Patterns Serisi

| # | Pattern | Kategori | Durum |
|---|---|---|---|
| 1 | Strategy | Behavioral | ✅ Tamamlandı |
| 2 | Command | Behavioral | ✅ Tamamlandı |
| 3 | Iterator | Behavioral | ✅ Tamamlandı |
| 4 | Template Metot | Behavioral | 🔜 Yakında |
| 5 | Observer | Behavioral | 🔜 Yakında |
| 6 | Memento | Behavioral | 🔜 Yakında |
| 7 | Mediator | Behavioral | 🔜 Yakında |
| 8 | Chain Of Responsibility | Behavioral | 🔜 Yakında |
| 9 | Visitor | Behavioral | 🔜 Yakında |
| 10 | State | Behavioral | 🔜 Yakında |

using FluentAssertions;
using Iterator_Implementation.Collections;
using Iterator_Implementation.Interfaces;
using Iterator_Implementation.Iterators;
using Iterator_Implementation.Models;
using Iterator_Implementation.Player;
using Moq;

namespace Iterator_Tests
{
    public class PlaylistIteratorTests
    {
        // --- Yardımcı metot ---
        private static Song CreateSong(
            string title = "Test Şarkısı",
            string artist = "Test Sanatçı",
            int duration = 180,
            bool isFavorite = false)
            => new(title, artist, duration, isFavorite);

        // 1. Happy Path — SequentialIterator

        [Fact]
        public void SequentialIterator_WithSongs_ShouldIterateInOrder()
        {
            var songs = new List<Song>
        {
            CreateSong("Song A"),
            CreateSong("Song B"),
            CreateSong("Song C")
        };
            var iterator = new SequentialIterator(songs);

            var results = new List<string>();
            while (iterator.HasNext())
                results.Add(iterator.Next().Title);

            results.Should().Equal("Song A", "Song B", "Song C");
        }

        [Fact]
        public void SequentialIterator_WhenExhausted_ShouldReturnFalseFromHasNext()
        {
            var songs = new List<Song> { CreateSong() };
            var iterator = new SequentialIterator(songs);

            iterator.Next(); // tüket

            iterator.HasNext().Should().BeFalse();
        }

        [Fact]
        public void SequentialIterator_AfterReset_ShouldStartFromBeginning()
        {
            var songs = new List<Song> { CreateSong("Song A"), CreateSong("Song B") };
            var iterator = new SequentialIterator(songs);

            iterator.Next();
            iterator.Next(); // tüket
            iterator.Reset();

            iterator.HasNext().Should().BeTrue();
            iterator.Next().Title.Should().Be("Song A");
        }

        // 2. Alt Sistem Çağrı Doğrulama — ShuffleIterator

        [Fact]
        public void ShuffleIterator_WithSeed_ShouldIterateAllSongsExactlyOnce()
        {
            var songs = new List<Song>
        {
            CreateSong("Song A"),
            CreateSong("Song B"),
            CreateSong("Song C"),
            CreateSong("Song D")
        };
            var iterator = new ShuffleIterator(songs, seed: 42);

            var results = new List<string>();
            while (iterator.HasNext())
                results.Add(iterator.Next().Title);

            results.Should().HaveCount(4);
            results.Should().BeEquivalentTo(new[] { "Song A", "Song B", "Song C", "Song D" });
        }

        [Fact]
        public void ShuffleIterator_AfterReset_ShouldReplayTheSameShuffledOrder()
        {
            var songs = new List<Song>
        {
            CreateSong("Song A"),
            CreateSong("Song B"),
            CreateSong("Song C")
        };
            var iterator = new ShuffleIterator(songs, seed: 42);

            var firstRun = new List<string>();
            while (iterator.HasNext())
                firstRun.Add(iterator.Next().Title);

            iterator.Reset();

            var secondRun = new List<string>();
            while (iterator.HasNext())
                secondRun.Add(iterator.Next().Title);

            // Reset sonrası aynı karışık sırayla başa dönmeli
            secondRun.Should().Equal(firstRun);
        }

        // 3. FavoriteIterator

        [Fact]
        public void FavoriteIterator_WithMixedSongs_ShouldReturnOnlyFavorites()
        {
            var songs = new List<Song>
        {
            CreateSong("Song A", isFavorite: true),
            CreateSong("Song B", isFavorite: false),
            CreateSong("Song C", isFavorite: true),
            CreateSong("Song D", isFavorite: false)
        };
            var iterator = new FavoriteIterator(songs);

            var results = new List<string>();
            while (iterator.HasNext())
                results.Add(iterator.Next().Title);

            results.Should().Equal("Song A", "Song C");
            results.Should().HaveCount(2);
        }

        [Fact]
        public void FavoriteIterator_WithNoFavorites_ShouldHaveNoNext()
        {
            var songs = new List<Song>
        {
            CreateSong("Song A", isFavorite: false),
            CreateSong("Song B", isFavorite: false)
        };
            var iterator = new FavoriteIterator(songs);

            iterator.HasNext().Should().BeFalse();
        }

        // 4. MusicPlayer — Moq ile doğrulama

        [Fact]
        public void Play_WithEmptyIterator_ShouldReturnFailResult()
        {
            var mockIterator = new Mock<IIterator<Song>>();
            mockIterator.Setup(i => i.HasNext()).Returns(false);

            var player = new MusicPlayer();
            var result = player.Play(mockIterator.Object);

            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Contain("bulunamadı");
            result.PlayedSongs.Should().BeEmpty();
            result.TotalDurationSeconds.Should().Be(0);
        }

        [Fact]
        public void Play_WhenPlaying_ShouldCallNextExactlyOncePerSong()
        {
            var queue = new Queue<Song>(new[]
            {
            CreateSong("Song A", duration: 100),
            CreateSong("Song B", duration: 150),
            CreateSong("Song C", duration: 200)
        });

            var mockIterator = new Mock<IIterator<Song>>();
            mockIterator.Setup(i => i.HasNext()).Returns(() => queue.Count > 0);
            mockIterator.Setup(i => i.Next()).Returns(() => queue.Dequeue());

            var player = new MusicPlayer();
            var result = player.Play(mockIterator.Object);

            result.TotalDurationSeconds.Should().Be(450);
            result.PlayedSongs.Should().HaveCount(3);
            mockIterator.Verify(i => i.Next(), Times.Exactly(3));
        }

        // 5. Playlist — Koleksiyon testleri

        [Fact]
        public void Playlist_AddSong_ShouldIncreaseCount()
        {
            var playlist = new Playlist("My Playlist");
            playlist.AddSong(CreateSong("Song A"));
            playlist.AddSong(CreateSong("Song B"));

            playlist.Count.Should().Be(2);
        }

        [Fact]
        public void Playlist_CreateSequentialIterator_ShouldIterateAllAddedSongs()
        {
            var playlist = new Playlist("My Playlist");
            playlist.AddSong(CreateSong("Song A"));
            playlist.AddSong(CreateSong("Song B"));

            var iterator = playlist.CreateSqeuentialIterator();
            var results = new List<string>();
            while (iterator.HasNext())
                results.Add(iterator.Next().Title);

            results.Should().Equal("Song A", "Song B");
        }

        // 6. Başarısız Senaryo — iterator sonundan Next()

        [Fact]
        public void SequentialIterator_Next_WhenExhausted_ShouldThrowInvalidOperationException()
        {
            var iterator = new SequentialIterator(new List<Song>());

            var act = () => iterator.Next();

            act.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void FavoriteIterator_Next_WhenNoFavorites_ShouldThrowInvalidOperationException()
        {
            var iterator = new FavoriteIterator(new List<Song> { CreateSong(isFavorite: false) });

            var act = () => iterator.Next();

            act.Should().Throw<InvalidOperationException>();
        }

        // 7. Guard Clause — Song

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Song_WithNullOrWhiteSpaceTitle_ShouldThrowArgumentException(string? title)
        {
            var act = () => new Song(title!, "Sanatçı", 180);

            act.Should().Throw<ArgumentException>().WithParameterName("title");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Song_WithNullOrWhiteSpaceArtist_ShouldThrowArgumentException(string? artist)
        {
            var act = () => new Song("Başlık", artist!, 180);

            act.Should().Throw<ArgumentException>().WithParameterName("artist");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100)]
        public void Song_WithNonPositiveDuration_ShouldThrowArgumentOutOfRangeException(int duration)
        {
            var act = () => new Song("Başlık", "Sanatçı", duration);

            act.Should().Throw<ArgumentOutOfRangeException>().WithParameterName("durationSeconds");
        }

        // 8. Guard Clause — Playlist

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Playlist_WithNullOrWhiteSpaceName_ShouldThrowArgumentException(string? name)
        {
            var act = () => new Playlist(name!);

            act.Should().Throw<ArgumentException>().WithParameterName("name");
        }

        // 9. Constructor Null Guard — Iterator'lar

        [Fact]
        public void SequentialIterator_Constructor_WithNullSongs_ShouldThrowArgumentNullException()
        {
            var act = () => new SequentialIterator(null!);

            act.Should().Throw<ArgumentNullException>().WithParameterName("songs");
        }

        [Fact]
        public void ShuffleIterator_Constructor_WithNullSongs_ShouldThrowArgumentNullException()
        {
            var act = () => new ShuffleIterator(null!);

            act.Should().Throw<ArgumentNullException>().WithParameterName("songs");
        }

        [Fact]
        public void FavoriteIterator_Constructor_WithNullSongs_ShouldThrowArgumentNullException()
        {
            var act = () => new FavoriteIterator(null!);

            act.Should().Throw<ArgumentNullException>().WithParameterName("songs");
        }

        [Fact]
        public void MusicPlayer_Play_WithNullIterator_ShouldThrowArgumentNullException()
        {
            var player = new MusicPlayer();
            var act = () => player.Play(null!);

            act.Should().Throw<ArgumentNullException>().WithParameterName("iterator");
        }

        // 10. PlaylistResult — Factory metot testleri

        [Fact]
        public void PlaylistResult_Success_ShouldHaveCorrectProperties()
        {
            var songs = new List<string> { "Queen - Bohemian Rhapsody", "Eagles - Hotel California" };
            var result = PlaylistResult.Success(songs, 745);

            result.IsSuccess.Should().BeTrue();
            result.Message.Should().Be("Playlist başarıyla çalındı.");
            result.PlayedSongs.Should().HaveCount(2);
            result.TotalDurationSeconds.Should().Be(745);
            result.TracingCode.Should().NotBeNullOrEmpty();
            result.TracingCode.Should().HaveLength(8);
        }

        [Fact]
        public void PlaylistResult_Fail_ShouldHaveIsSuccessFalse()
        {
            var result = PlaylistResult.Fail("Çalınacak şarkı bulunamadı.");

            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Be("Çalınacak şarkı bulunamadı.");
            result.PlayedSongs.Should().BeEmpty();
            result.TotalDurationSeconds.Should().Be(0);
            result.TracingCode.Should().BeEmpty();
        }
    }
}
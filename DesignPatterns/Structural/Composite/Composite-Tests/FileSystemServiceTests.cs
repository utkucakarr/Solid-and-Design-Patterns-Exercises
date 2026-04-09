using Composite_İmplementation.Services;
using FluentAssertions;
using Directory = Composite_İmplementation.Components.Directory;
using File = Composite_İmplementation.Components.File;

namespace Composite_Tests
{
    public class FileSystemServiceTests
    {
        private readonly FileSystemService _sut = new();

        private Directory CreateSampleFileSystem()
        {
            var root = new Directory("root");
            var docs = new Directory("docs");

            docs.Add(new File("cv.pdf", 102_400));
            docs.Add(new File("cover.doc", 51_200));

            var photos = new Directory("photos");
            photos.Add(new File("photo1.jpg", 2_048_000));
            photos.Add(new File("photo2.jpg", 1_024_000));

            root.Add(docs);
            root.Add(photos);
            root.Add(new File("readme.txt", 1_024));

            return root;
        }

        // --- CalculateTotalSize Testleri ---
        [Fact]
        public void CalculateTotalSize_ForFile_ShouldReturnFileSize()
        {
            var file = new File("test.txt", 1024);

            _sut.CalculateTotalSize(file).Should().Be(1024);
        }

        [Fact]
        public void CalculateTotalSize_ForDirectory_ShouldReturnRecursiveSum()
        {
            var root = CreateSampleFileSystem();
            var expected = 102_400 + 51_200 + 2_048_000 + 1_024_000 + 1_024;

            _sut.CalculateTotalSize(root).Should().Be(expected);
        }

        [Fact]
        public void CalculateTotalSize_WithNullItem_ShouldThrowArgumentNullException()
        {
            var act = () => _sut.CalculateTotalSize(null!);

            act.Should().Throw<ArgumentNullException>();
        }

        // --- FindByName Testleri ---
        [Fact]
        public void FindByName_ExistingFile_ShouldReturnItem()
        {
            var root = CreateSampleFileSystem();
            var result = _sut.FindByName(root, "cv.pdf");

            result.Should().NotBeNull();
            result.Name.Should().Be("cv.pdf");
        }

        [Fact]
        public void FindByName_NonExistingItem_ShouldReturnNull()
        {
            var root = CreateSampleFileSystem();
            var result = _sut.FindByName(root, "nonexistent.txt");

            result.Should().BeNull();
        }

        [Fact]
        public void FindByName_CaseInsensitive_ShouldReturnItem()
        {
            var root = CreateSampleFileSystem();
            var result = _sut.FindByName(root, "CV.PDF");

            result.Should().NotBeNull();
        }

        // --- FindAll Testleri ---

        [Fact]
        public void FindAll_WithSizePredicate_ShouldReturnMatchingFiles()
        {
            var root = CreateSampleFileSystem();
            var result = _sut.FindAll(
                root,
                item => item is File f && f.SizeInBytes > 1_000_000);

            result.Should().HaveCount(2);
        }

        [Fact]
        public void FindAll_WithNamePredicate_ShouldReturnMatchingItems()
        {
            var root = CreateSampleFileSystem();
            var result = _sut.FindAll(
                root,
                item => item.Name.EndsWith(".jpg"));

            result.Should().HaveCount(2);
        }

        [Fact]
        public void FindAll_WithNoMatch_ShouldReturnEmptyList()
        {
            var root = CreateSampleFileSystem();
            var result = _sut.FindAll(
                root,
                item => item.Name.EndsWith(".mp4"));

            result.Should().BeEmpty();
        }

        [Fact]
        public void CalculateTotalSize_FileAndDirectory_SameInterface()
        {
            // Composite garantisi — dosya ve klasör aynı şekilde işleniyor
            var file = new File("test.txt", 500);
            var dir = new Directory("docs");
            dir.Add(new File("a.txt", 300));
            dir.Add(new File("b.txt", 200));

            // İkisi de aynı interface — ayrı kod yok!
            _sut.CalculateTotalSize(file).Should().Be(500);
            _sut.CalculateTotalSize(dir).Should().Be(500);
        }
    }
}
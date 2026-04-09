using Composite_Ýmplementation.Components;
using Composite_Ýmplementation.Interfaces;
using FluentAssertions;

namespace Composite_Tests
{
    public class FileTests
    {
        // --- Constructure Testleri ---

        [Fact]
        public void Constructor_ShouldSetProperties_Correctly()
        {
            var file = new Composite_Ýmplementation.Components.File("test.txt", 1024);

            file.Name.Should().Be("test.txt");
            file.SizeInBytes.Should().Be(1024);
        }

        [Fact]
        public void Constructor_WithEmptyName_ShouldThrowArgumentException()
        {
            var act = () => new Composite_Ýmplementation.Components.File("", 1024);

            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Constructor_WithNegaticeSize_ShouldThrowArgumentOutOfRangeException()
        {
            var act = () => new Composite_Ýmplementation.Components.File("test.text", -1);

            act.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void GetSize_ShouldReturnOwnSize()
        {
            var file = new Composite_Ýmplementation.Components.File("test.txt", 2048);

            file.GetSize().Should().Be(2048);
        }

        [Fact]
        public void GetSize_WithZeroBytes_ShouldReturnZero()
        {
            var file = new Composite_Ýmplementation.Components.File("test.txt", 0);

            file.GetSize().Should().Be(0);
        }

        [Fact]
        public void GetParent_Initially_ShouldReturnNull()
        {
            var file = new Composite_Ýmplementation.Components.File("test.txt", 2048);

            file.GetParent().Should().BeNull();
        }

        [Fact]
        public void SetParent_ShouldUpdateParent()
        {
            var file = new Composite_Ýmplementation.Components.File("test.txt", 1024);
            var directory = new Composite_Ýmplementation.Components.Directory("docs");

            file.SetParent(directory);

            file.GetParent().Should().BeSameAs(directory);
        }

        // --- Interface Garantisi ---
        [Fact]
        public void File_ShouldImplement_IFileSystemItem()
        {
            var file = new Composite_Ýmplementation.Components.File("test.txt", 1024);

            file.Should().BeAssignableTo<IFileSystemItem>();
        }

        // --- Print Testleri ---
        [Fact]
        public void Print_ShouldNotThrow()
        {
            var file = new Composite_Ýmplementation.Components.File("test.txt", 1024);

            var act = () => file.Print();

            act.Should().NotThrow();
        }
    }
}
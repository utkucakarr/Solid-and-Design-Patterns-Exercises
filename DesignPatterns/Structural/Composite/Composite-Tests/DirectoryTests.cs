using Composite_İmplementation.Interfaces;
using FluentAssertions;
using Directory = Composite_İmplementation.Components.Directory;
using File = Composite_İmplementation.Components.File;

namespace Composite_Tests
{
    public class DirectoryTests
    {
        // Constructor Testleri ---
        [Fact]
        public void Contructor_ShouldSetName_Correctly()
        {
            var directory = new Directory("docs");

            directory.Name.Should().Be("docs");
        }

        [Fact]
        public void Constructor_WithEmptyName_ShouldThrowArgumentException()
        {
            var act = () => new Directory(" ");

            act.Should().Throw<ArgumentException>();  
        }


        // --- Add / Remove Testleri ---
        [Fact]
        public void Add_ShouldIncreaseChildCount()
        {
            var directory = new Directory("docs");
            var file = new File("test.txt", 1024);

            directory.Add(file);

            directory.ChildCount.Should().Be(1);
        }

        [Fact]
        public void Add_ShouldSetParent_OnChild()
        {
            var directory = new Directory("docs");
            var file = new File("test.txt", 1024);

            directory.Add(file);

            file.GetParent().Should().BeSameAs(directory);
        }

        [Fact]
        public void Add_WithNull_ShouldThrowArgumentNullException()
        {
            var directory = new Directory("docs");

            var act = () => directory.Add(null);

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Remove_ShouldClearParent_OnChild()
        {
            var dir = new Directory("docs");
            var file = new File("test.txt", 1024);

            dir.Add(file);
            dir.Remove(file);

            file.GetParent().Should().BeNull();
        }

        [Fact]
        public void Remove_ShouldDecreaseChildCount()
        {
            var directory = new Directory("docs");
            var file = new File("test.txt", 2048);

            directory.Add(file);
            directory.Remove(file);

            directory.ChildCount.Should().Be(0);
        }

        // --- GetSize Testleri ---
        [Fact]
        public void GetSize_WithFiles_ShouldReturnSumOfFileSizes()
        {
            var dir = new Directory("docs");
            dir.Add(new File("a.txt", 100));
            dir.Add(new File("b.txt", 200));
            dir.Add(new File("c.txt", 300));

            dir.GetSize().Should().Be(600);
        }

        [Fact]
        public void GetSize_WithNestedDirectories_ShouldReturnRecursiveSum()
        {
            // Composite'ın asıl gücü - recursive hesaplama
            var root = new Directory("root");
            var sub = new Directory("sub");

            root.Add(new File("a.txt", 100));
            sub.Add(new File("b.txt", 200));
            sub.Add(new File("c.txt", 300));
            root.Add(sub);

            // root: 100 + sub(200 + 300) = 600
            root.GetSize().Should().Be(600);
        }

        [Fact]
        public void GetSize_WithDeeplyNestedDirectories_ShouldReturnCorrectSum()
        {
            var level1 = new Directory("L1");
            var level2 = new Directory("L2");
            var level3 = new Directory("L3");

            level3.Add(new File("deep.txt", 1000));
            level2.Add(level3);
            level1.Add(level2);

            level1.GetSize().Should().Be(1000);
        }

        // --- Interface Garantisi ---
        [Fact]
        public void Directory_ShouldImplement_IFileSystem()
        {
            var directory = new Directory("docs");

            directory.Should().BeAssignableTo<IFileSystemItem>();
        }

        [Fact]
        public void Directory_CanContain_BothFilesAndDirectories()
        {
            // Composite garantisi — aynı interface, farklı tipler
            var dir = new Directory("mixed");
            var file = new File("test.txt", 1024);
            var subDir = new Directory("sub");

            dir.Add(file);
            dir.Add(subDir);

            dir.ChildCount.Should().Be(2);
            dir.Contain(file).Should().BeTrue();
            dir.Contain(subDir).Should().BeTrue();
        }
    }
}

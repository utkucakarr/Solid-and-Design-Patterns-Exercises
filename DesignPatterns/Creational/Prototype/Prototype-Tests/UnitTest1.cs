using FluentAssertions;
using Prototype_Implementation.Documents;
using Prototype_Implementation.Models;

namespace Prototype_Tests
{
    public class ReportDocumentTests
    {
        private ReportDocument CreateTemplate() => new(
                title: "Test Raporu",
                content: "Test içeriði",
                tableData: new List<string> { "A", "B", "C" },
                metadata: new DocumentMetadata("Yazar", "Departman", "v1.0")
            );

        // --- Clone (Sýð Kopya) Testleri ---
        [Fact]
        public void Clone_ShouldReturnNewInstance()
        {
            var orginal = CreateTemplate();
            var clone = orginal.Clone();

            clone.Should().NotBeSameAs(orginal);
        }

        [Fact]
        public void Clone_ShouldCopyTitle()
        {
            var orginal = CreateTemplate();
            var clone = orginal.Clone();

            clone.Title.Should().Be(orginal.Title);
        }

        [Fact]
        public void Clone_ShouldShareTableDataReference()
        {
            // Sýð kopya — ayný liste referansý paylaþýlýyor
            var orginal = CreateTemplate();
            var clone = orginal.Clone();

            clone.TableData.Should().BeSameAs(orginal.TableData);
        }

        [Fact]
        public void Clone_WhenTableDataModified_ShouldAffectOrginal()
        {
            var orginal = CreateTemplate();
            var clone = orginal.Clone();
            var orginalCount = orginal.TableData.Count();

            clone.TableData.Add("Yeni");
            orginal.TableData.Count.Should().Be(orginalCount + 1);
        }

        // --- DeepClone (Derin Kopya) Testleri ---
        [Fact]
        public void DeepClone_ShouldReturnNewInstance()
        {
            var orginal = CreateTemplate();
            var deepClone = orginal.DeepClone();

            deepClone.Should().NotBeSameAs(orginal);
        }

        [Fact]
        public void DeepClone_ShouldCopyAllProperties()
        {
            var orginal = CreateTemplate();
            var deepClone = orginal.DeepClone();

            deepClone.Title.Should().Be(orginal.Title);
            deepClone.Content.Should().Be(orginal.Content);
            deepClone.DocumentType.Should().Be(orginal.DocumentType);
        }

        [Fact]
        public void DeepClone_ShouldNotShareTableDataReference()
        {
            var orginal = CreateTemplate();
            var deepClone = orginal.DeepClone();

            deepClone.TableData.Should().NotBeSameAs(orginal.TableData);
        }

        [Fact]
        public void DeepClone_WhenTableDataModified_ShouldNotAffectOriginal()
        {
            // Derin kopyanýn en önemli garantisi
            var original = CreateTemplate();
            var deepClone = original.DeepClone();
            var originalCount = original.TableData.Count;

            deepClone.TableData.Add("Yeni");

            original.TableData.Count.Should().Be(originalCount);
        }

        [Fact]
        public void DeepClone_ShouldNotShareMetadataReference()
        {
            var original = CreateTemplate();
            var deepClone = original.DeepClone();

            deepClone.Metadata.Should().NotBeSameAs(original.Metadata);
        }

        // --- Guard Clause Testleri ---

        [Fact]
        public void Constructor_WithEmptyTitle_ShouldThrowArgumentException()
        {
            var act = () => new ReportDocument(
                " ",
                "içerik",
                new List<string>(),
                new DocumentMetadata("Yazar", "Dept", "v1"));

            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Constructor_WithNullTableData_ShouldThrowArgumentNullException()
        {
            var act = () => new ReportDocument(
                "Baþlýk",
                "içerik",
                null!,
                new DocumentMetadata("Yazar", "Dept", "v1"));

            act.Should().Throw<ArgumentNullException>();
        }
    }
}
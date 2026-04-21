using FluentAssertions;
using TemplateMethod_Implementation.Reports;

namespace TemplateMethod_Tests
{
    public class ReportGeneratorTests
    {
        private readonly string[] _validData = ["Ocak: 12.000₺", "Şubat: 15.500₺", "Mart: 9.800₺"];
        private readonly PdfReportGenerator _pdf = new();
        private readonly ExcelReportGenerator _excel = new();
        private readonly CsvReportGenerator _csv = new();

        // 1. Happy Path — Başarılı senaryolar

        [Fact]
        public void Generate_PdfWithValidData_ShouldReturnSuccessResult()
        {
            var result = _pdf.Generate("Q1 Raporu", _validData);

            result.IsSuccess.Should().BeTrue();
            result.Format.Should().Be("PDF");
            result.Content.Should().Contain("[PDF HEADER]");
            result.Content.Should().Contain("Q1 Raporu");
            result.Content.Should().Contain("[PDF FOOTER]");
            result.Message.Should().Contain("Q1 Raporu");
        }

        [Fact]
        public void Generate_ExcelWithValidData_ShouldReturnSuccessResult()
        {
            var result = _excel.Generate("Q1 Raporu", _validData);

            result.IsSuccess.Should().BeTrue();
            result.Format.Should().Be("Excel");
            result.Content.Should().Contain("HÜCRE[A1]");
            result.Content.Should().Contain("Q1 Raporu");
            result.Content.Should().Contain("HÜCRE[Son]");
        }

        [Fact]
        public void Generate_CsvWithValidData_ShouldReturnSuccessResult()
        {
            var result = _csv.Generate("Q1 Raporu", _validData);

            result.IsSuccess.Should().BeTrue();
            result.Format.Should().Be("CSV");
            result.Content.Should().Contain("Başlık,Değer");
            result.Content.Should().Contain("Q1 Raporu");
            result.Content.Should().Contain("kayıt dışa aktarıldı");
        }

        [Fact]
        public void Generate_AllFormats_ShouldContainAllDataRows()
        {
            var result = _pdf.Generate("Test", _validData);

            foreach (var row in _validData)
                result.Content.Should().Contain(row);
        }

        [Fact]
        public void Generate_ShouldSetGeneratedAtToUtcNow()
        {
            var before = DateTime.UtcNow.AddSeconds(-1);
            var result = _pdf.Generate("Test", _validData);
            var after = DateTime.UtcNow.AddSeconds(1);

            result.GeneratedAt.Should().BeAfter(before).And.BeBefore(after);
        }

        // 2. Template Method — Algoritma sırası doğrulama

        [Fact]
        public void Generate_Pdf_ShouldHaveHeaderBeforeFooter()
        {
            var result = _pdf.Generate("Sıra Testi", _validData);

            var headerIndex = result.Content!.IndexOf("[PDF HEADER]", StringComparison.Ordinal);
            var footerIndex = result.Content!.IndexOf("[PDF FOOTER]", StringComparison.Ordinal);

            headerIndex.Should().BeLessThan(footerIndex,
                because: "Template Method algoritma sırasını garanti eder: header, rows, footer");
        }

        [Fact]
        public void Generate_Excel_ShouldHaveA1BeforeRowsBeforeSummary()
        {
            var result = _excel.Generate("Sıra Testi", _validData);

            var a1Index = result.Content!.IndexOf("HÜCRE[A1]", StringComparison.Ordinal);
            var a3Index = result.Content!.IndexOf("HÜCRE[A3]", StringComparison.Ordinal);
            var sonIndex = result.Content!.IndexOf("HÜCRE[Son]", StringComparison.Ordinal);

            a1Index.Should().BeLessThan(a3Index);
            a3Index.Should().BeLessThan(sonIndex);
        }

        [Fact]
        public void Generate_Csv_ShouldHaveHeaderLineFirst()
        {
            var result = _csv.Generate("Sıra Testi", _validData);

            var lines = result.Content!.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            lines[0].Trim().Should().Be("Başlık,Değer",
                because: "CSV'de ilk satır her zaman header olmalıdır");
        }

        // 3. Başarısız senaryolar — boş veri

        [Fact]
        public void Generate_WithEmptyData_ShouldReturnFailResult()
        {
            var result = _pdf.Generate("Boş Rapor", Array.Empty<string>());

            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Contain("boş olamaz");
            result.Content.Should().BeNull();
            result.Format.Should().BeNull();
        }

        [Fact]
        public void Generate_Excel_WithEmptyData_ShouldReturnFailResult()
        {
            var result = _excel.Generate("Boş Rapor", Array.Empty<string>());

            result.IsSuccess.Should().BeFalse();
            result.Content.Should().BeNull();
        }

        [Fact]
        public void Generate_Csv_WithEmptyData_ShouldReturnFailResult()
        {
            var result = _csv.Generate("Boş Rapor", Array.Empty<string>());

            result.IsSuccess.Should().BeFalse();
        }

        [Fact]
        public void Fail_ShouldSetIsSuccessFalseAndMessage()
        {
            var result = TemplateMethod_Implementation.Models.ReportResult.Fail("test hatası");

            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Be("test hatası");
            result.Content.Should().BeNull();
            result.Format.Should().BeNull();
        }

        // 4. Guard clause testleri

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null!)]
        public void Generate_Pdf_WithNullOrWhiteSpaceTitle_ShouldThrowArgumentException(string? title)
        {
            var act = () => _pdf.Generate(title!, _validData);

            act.Should().Throw<ArgumentException>()
               .WithParameterName("reportTitle");
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null!)]
        public void Generate_Excel_WithNullOrWhiteSpaceTitle_ShouldThrowArgumentException(string? title)
        {
            var act = () => _excel.Generate(title!, _validData);

            act.Should().Throw<ArgumentException>()
               .WithParameterName("reportTitle");
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null!)]
        public void Generate_Csv_WithNullOrWhiteSpaceTitle_ShouldThrowArgumentException(string? title)
        {
            var act = () => _csv.Generate(title!, _validData);

            act.Should().Throw<ArgumentException>()
               .WithParameterName("reportTitle");
        }

        [Fact]
        public void Generate_Pdf_WithNullData_ShouldThrowArgumentNullException()
        {
            var act = () => _pdf.Generate("Başlık", null!);

            act.Should().Throw<ArgumentNullException>()
               .WithParameterName("data");
        }

        [Fact]
        public void Generate_Excel_WithNullData_ShouldThrowArgumentNullException()
        {
            var act = () => _excel.Generate("Başlık", null!);

            act.Should().Throw<ArgumentNullException>()
               .WithParameterName("data");
        }

        [Fact]
        public void Generate_Csv_WithNullData_ShouldThrowArgumentNullException()
        {
            var act = () => _csv.Generate("Başlık", null!);

            act.Should().Throw<ArgumentNullException>()
               .WithParameterName("data");
        }

        // 5. FormatName doğrulaması

        [Fact]
        public void Generate_Pdf_ShouldReturnPdfFormatName()
        {
            var result = _pdf.Generate("Test", _validData);
            result.Format.Should().Be("PDF");
        }

        [Fact]
        public void Generate_Excel_ShouldReturnExcelFormatName()
        {
            var result = _excel.Generate("Test", _validData);
            result.Format.Should().Be("Excel");
        }

        [Fact]
        public void Generate_Csv_ShouldReturnCsvFormatName()
        {
            var result = _csv.Generate("Test", _validData);
            result.Format.Should().Be("CSV");
        }

        // 6. Mesaj içerik doğrulaması

        [Fact]
        public void Generate_Success_ShouldIncludeReportTitleInMessage()
        {
            var result = _pdf.Generate("Yıllık Bütçe", _validData);

            result.Message.Should().Contain("Yıllık Bütçe");
            result.Message.Should().Contain("PDF");
        }

        [Fact]
        public void Generate_Success_ShouldReturnNonEmptyContent()
        {
            var result = _csv.Generate("Test Raporu", _validData);

            result.Content.Should().NotBeNullOrWhiteSpace();
        }
    }
}
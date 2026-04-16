using Bridge_Implementation.Interfaces;
using Bridge_Implementation.Renderers;
using Bridge_Implementation.Report;
using FluentAssertions;

namespace Bridge_Tests
{
    public class BridgeIndependenceTests
    {
        // Aynı rapor farklı renderer
        [Fact]
        public void SalesReport_WithPdfRenderer_ShouldSucceed()
        {
            var report = new SalesReport(new PdfReportRenderer());
            var result = report.Generate();

            result.IsSuccess.Should().BeTrue();
            result.RendererName.Should().Be("PDF");
            result.ReportName.Should().Be("Satış Raporu");
        }

        [Fact]
        public void SalesReport_WithExcelRenderer_ShouldSucceed()
        {
            var report = new SalesReport(new ExcelReportRenderer());
            var result = report.Generate();

            result.IsSuccess.Should().BeTrue();
            result.RendererName.Should().Be("Excel");
            result.ReportName.Should().Be("Satış Raporu");
        }

        [Fact]
        public void SalesReport_WithCsvRenderer_ShouldSucceed()
        {
            var report = new SalesReport(new CsvReportRenderer());
            var result = report.Generate();

            result.IsSuccess.Should().BeTrue();
            result.RendererName.Should().Be("CSV");
            result.ReportName.Should().Be("Satış Raporu");
        }

        // Farklı rapor - Aynı Renderer 
        [Fact]
        public void AllReports_WithPdfRenderer_ShouldSucceed()
        {
            IReportRenderer pdf = new PdfReportRenderer();

            var reports = new IReport[]
            {
                new SalesReport(pdf),
                new StockReport(pdf),
                new FinanceReport(pdf)
            };

            foreach (var report in reports)
            {
                var result = report.Generate();
                result.IsSuccess.Should().BeTrue();
                result.RendererName.Should().Be("PDF");
            }
        }

        // Bridge Bağımsızlık Garantisi

        [Fact]
        public void ReportName_ShouldNotDependOnRenderer()
        {
            // Aynı rapor farklı renderer'larla aynı ReportName döndürmeli
            var salesPdf = new SalesReport(new PdfReportRenderer());
            var salesExcel = new SalesReport(new ExcelReportRenderer());
            var salesCsv = new SalesReport(new CsvReportRenderer());

            salesPdf.ReportName.Should().Be(salesExcel.ReportName);
            salesExcel.ReportName.Should().Be(salesCsv.ReportName);
        }

        [Fact]
        public void AllCombinations_ShouldSucceed()
        {
            // 3 rapor × 3 renderer = 9 kombinasyon — hepsi çalışmalı
            var renderers = new IReportRenderer[]
            {
            new PdfReportRenderer(),
            new ExcelReportRenderer(),
            new CsvReportRenderer()
            };

            foreach (var renderer in renderers)
            {
                var reports = new IReport[]
                {
                new SalesReport(renderer),
                new StockReport(renderer),
                new FinanceReport(renderer)
                };

                foreach (var report in reports)
                {
                    var result = report.Generate();
                    result.IsSuccess.Should().BeTrue();
                }
            }
        }
    }
}
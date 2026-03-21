using ISP_Violation;
using ISP_Implementation.Printers;
using ISP_Implementation.Interfaces;

//Console.WriteLine("╔══════════════════════════════════════╗");
//Console.WriteLine("║      ISP İHLALİ — CANLI DEMO         ║");
//Console.WriteLine("╚══════════════════════════════════════╝\n");

//var badPrinters = new List<IPrinter>
//{
//    new ISP_Violation.AllInOnePrinter(),
//    new ISP_Violation.OfficePrinter(),
//    new ISP_Violation.BasicPrinter()
//};

//Console.WriteLine("─── Yazdırma İşlemi (Sorunsuz) ───");
//foreach (var printer in badPrinters)
//    printer.Print("Rapor.pdf");

//Console.WriteLine();
//Console.WriteLine("─── Tarama İşlemi Başlatılıyor ───");
//Console.WriteLine("Listede BasicPrinter var...\n");

//try
//{
//    foreach (var printer in badPrinters)
//        printer.Scan("Rapor.pdf");
//}
//catch (NotSupportedException ex)
//{
//    Console.WriteLine($"\n RUNTIME HATASI: {ex.Message}");
//    Console.WriteLine("   -> AllInOnePrinter taradı.");
//    Console.WriteLine("   -> OfficePrinter taradı.");
//    Console.WriteLine("   -> BasicPrinter'a gelince program ÇÖKTÜ!\n");
//}

Console.WriteLine("╔══════════════════════════════════════╗");
Console.WriteLine("║      ISP ÇÖZÜMÜ — DOĞRU YAKLAŞIM     ║");
Console.WriteLine("╚══════════════════════════════════════╝\n");

var basicPrinter = new ISP_Implementation.Printers.BasicPrinter();
var officePrinter = new ISP_Implementation.Printers.OfficePrinter();
var allInOne = new ISP_Implementation.Printers.AllInOnePrinter();

// Tüm cihazlar yazdırabilir — güvenli!
var allPrintables = new List<IPrintable> { basicPrinter, officePrinter, allInOne };

Console.WriteLine("─── Yazdırma (Güvenli) ───");
foreach (var printer in allPrintables)
{
    var result = printer.Print("Rapor.pdf");
    Console.WriteLine(result.IsSuccess ? $" {result.Message}" : $"  {result.Message}");
}

Console.WriteLine();

// Sadece tarayabilenler — BasicPrinter giremez, compiler engeller!
var scannables = new List<IScannable> { officePrinter, allInOne };

Console.WriteLine("─── Tarama (Güvenli) ───");
Console.WriteLine("   BasicPrinter listeye giremez — compiler engeller!\n");

foreach (var scanner in scannables)
{
    var result = scanner.Scan("Sozlesme.pdf");
    Console.WriteLine(result.IsSuccess ? $" {result.Message}" : $"  {result.Message}");
}

Console.WriteLine();

// Sadece faks gönderebilenler
var faxables = new List<IFaxable> { allInOne };

Console.WriteLine("─── Faks (Güvenli) ───");
Console.WriteLine("   Sadece AllInOnePrinter bu listeye girebilir!\n");

foreach (var fax in faxables)
{
    var result = fax.Fax("Fatura.pdf");
    Console.WriteLine(result.IsSuccess ? $" {result.Message}" : $"  {result.Message}");
}

Console.WriteLine("\n─── SONUÇ ───");
Console.WriteLine("  Bad  -> Runtime'da patlıyor. BasicPrinter tarama listesine girebiliyor.");
Console.WriteLine("  Good -> Compiler koruyor. Her cihaz sadece yapabildiği listeye giriyor.\n");
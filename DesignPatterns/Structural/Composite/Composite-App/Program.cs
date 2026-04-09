using Composite_İmplementation.Services;
using Composite_Violation.FileSystem;

Console.WriteLine("╔══════════════════════════════════════╗");
Console.WriteLine("║   COMPOSITE İHLALİ — CANLI DEMO       ║");
Console.WriteLine("╚══════════════════════════════════════╝\n");

var manager = new FileSystemManagerBad();

var docs = new DirectoryBad("Documents");
docs.AddFile(new FileBad("cv.pdf", 102_400));
docs.AddFile(new FileBad("cover.docx", 51_200));

var photos = new DirectoryBad("Photos");
photos.AddFile(new FileBad("vacation.jpg", 2_048_000));
photos.AddFile(new FileBad("family.jpg", 1_024_000));

manager.Add(new FileBad("readme.txt", 1_024));
manager.Add(docs);
manager.Add(photos);

Console.WriteLine("--- Dosya Sistemi Yapısı ---\n");
manager.Print();

Console.WriteLine($"\n  Toplam boyut: {manager.CalculateTotalSize():N0} bytes");
Console.WriteLine();
Console.WriteLine(" Sorunlar:");
Console.WriteLine("  -> Dosya ve klasörler ayrı listede — tip kontrolü şart");
Console.WriteLine("  -> Her metotta if-else tip kontrolü tekrarlanıyor");
Console.WriteLine("  -> Yeni tip gelince tüm metodlara dokunmak gerekiyor\n");


Console.WriteLine("╔══════════════════════════════════════╗");
Console.WriteLine("║   COMPOSITE ÇÖZÜMÜ — DOĞRU YAKLAŞIM   ║");
Console.WriteLine("╚══════════════════════════════════════╝\n");

// Ağaç yapısı oluşturuluyor
var root = new Composite_İmplementation.Components.Directory("C:");

var documents = new Composite_İmplementation.Components.Directory("Documents");
documents.Add(new Composite_İmplementation.Components.File("cv.pdf", 102_400));
documents.Add(new Composite_İmplementation.Components.File("cover.docx", 51_200));

var work = new Composite_İmplementation.Components.Directory("Work");
work.Add(new Composite_İmplementation.Components.File("report.xlsx", 204_800));
work.Add(new Composite_İmplementation.Components.File("budget.xlsx", 102_400));
documents.Add(work);

var pictures = new Composite_İmplementation.Components.Directory("Pictures");
pictures.Add(new Composite_İmplementation.Components.File("vacation.jpg", 2_048_000));
pictures.Add(new Composite_İmplementation.Components.File("family.jpg", 1_024_000));

var downloads = new Composite_İmplementation.Components.Directory("Downloads");
downloads.Add(new Composite_İmplementation.Components.File("setup.exe", 5_120_000));
downloads.Add(new Composite_İmplementation.Components.File("archive.zip", 3_072_000));

root.Add(documents);
root.Add(pictures);
root.Add(downloads);
root.Add(new Composite_İmplementation.Components.File("boot.ini", 512));

// Client — tip kontrolü yok!
var service = new FileSystemService();

Console.WriteLine("--- Dosya Sistemi Yapısı ---\n");
service.PrintStructure(root);

Console.WriteLine($"\n  Toplam boyut: {service.CalculateTotalSize(root):N0} bytes");

Console.WriteLine("\n--- Arama: 'report.xlsx' ---\n");
var found = service.FindByName(root, "report.xlsx");
Console.WriteLine(found is not null
    ? $"  Bulundu: {found}"
    : "  Bulunamadı");

Console.WriteLine("\n--- 1MB'dan büyük dosyalar ---\n");
var largeFiles = service.FindAll(
    root,
    item => item is Composite_İmplementation.Components.File f && f.SizeInBytes > 1_000_000);

foreach (var item in largeFiles)
    Console.WriteLine($" {item.Name} ({((Composite_İmplementation.Components.File)item).SizeInBytes:N0} bytes)");

Console.WriteLine("\n--- SONUÇ ---");
Console.WriteLine("  Bad  -> Dosya/klasör ayrı — her yerde tip kontrolü");
Console.WriteLine("  Good -> Tek interface — tip kontrolü yok, recursive kolay\n");
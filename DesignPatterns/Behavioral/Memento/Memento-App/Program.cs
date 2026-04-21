#region ❌ VIOLATION — Geçmiş Document içinde, deep copy yok

using Memento_Implementation.Extensions;
using Memento_Implementation.Interfaces;
using Memento_Implementation.Models;
using Memento_Violation;
using Microsoft.Extensions.DependencyInjection;

Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
Console.WriteLine("║                VIOLATION YAKLAŞIMI                           ║");
Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");
Console.WriteLine();
Console.WriteLine(" Geçmiş yığını doğrudan Document_Bad içinde tutuluyor.");
Console.WriteLine(" Tags listesi referans kopyası — snapshot sonrası");
Console.WriteLine("  yapılan tag değişikliği geçmiş state'i de etkiliyor.");
Console.WriteLine(" Redo stack SetTitle/SetContent'te temizlenmiyor.");
Console.WriteLine(" SRP ihlali — Document hem içeriği hem geçmişi yönetiyor.");
Console.WriteLine();

var badDoc = new DocumentBad("İlk Başlık");

Console.WriteLine("▶ Başlangıç state:");
badDoc.PrintState();

Console.WriteLine();
Console.WriteLine(" Başlık 'Güncellenmiş Başlık' yapılıyor...");
badDoc.SetTitle("Güncellenmiş Başlık");
badDoc.PrintState();

Console.WriteLine();
Console.WriteLine(" İçerik ekleniyor...");
badDoc.SetContent("Bu bir test içeriğidir.");
badDoc.PrintState();

Console.WriteLine();
Console.WriteLine(" Tag ekleniyor: 'csharp'...");
badDoc.AddTag("csharp");
badDoc.PrintState();

Console.WriteLine();
Console.WriteLine(" DEEP COPY HATASI GÖSTERIMI:");
Console.WriteLine(" Snapshot alındıktan SONRA tag listesi dışarıdan değiştiriliyor...");
var snapshotTags = badDoc.GetUndoHistory().LastOrDefault()?.Tags;
if (snapshotTags != null)
{
    // Snapshot'taki Tags listesi referans kopyası olduğu için
    // dışarıdan değiştirilince geçmiş state de bozuluyor
    Console.WriteLine($"  Snapshot'taki tag sayısı (beklenen: 0): {snapshotTags.Count}");
}

Console.WriteLine();
Console.WriteLine(" Undo yapılıyor...");
badDoc.Undo();
badDoc.PrintState();

Console.WriteLine();
Console.WriteLine(" Undo yapılıyor...");
badDoc.Undo();
badDoc.PrintState();

Console.WriteLine();
Console.WriteLine(" SetTitle sonrası Redo stack temizlenmedi —");
Console.WriteLine("  yeni değişiklik yapıldıktan sonra Redo tutarsız davranır.");

#endregion

Console.WriteLine();
Console.WriteLine("══════════════════════════════════════════════════════════════");
Console.WriteLine();

#region ✅ IMPLEMENTATION — Observer Pattern ile temiz çözüm

Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
Console.WriteLine("║              IMPLEMENTATION YAKLAŞIMI                        ║");
Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");
Console.WriteLine();

// DI Container kurulumu
var services = new ServiceCollection();
services.AddDocumnetHistroy();
var provider = services.BuildServiceProvider();

var editor = provider.GetRequiredService<IDocumentEditor>();

// Başlangıç state
Console.WriteLine(" Başlangıç state:");
PrintResult(editor.GetCurrentState());

Console.WriteLine();

// Başlık değişikliği
Console.WriteLine(" Başlık 'Tasarım Desenleri Rehberi' yapılıyor...");
PrintResult(editor.SetTitle("Tasarım Desenleri Rehberi"));

Console.WriteLine();

// İçerik ekleme
Console.WriteLine(" İçerik ekleniyor...");
PrintResult(editor.SetContent(
    "Bu döküman C# tasarım desenlerini kapsamlı şekilde açıklamaktadır."));

Console.WriteLine();

// Tag ekleme
Console.WriteLine("'csharp' etiketi ekleniyor...");
PrintResult(editor.AddTag("csharp"));

Console.WriteLine();

Console.WriteLine("'design-patterns' etiketi ekleniyor...");
PrintResult(editor.AddTag("design-patterns"));

Console.WriteLine();

Console.WriteLine("'dotnet' etiketi ekleniyor...");
PrintResult(editor.AddTag("dotnet"));

Console.WriteLine();

// Undo zinciri
Console.WriteLine(" UNDO ZİNCİRİ");
Console.WriteLine();

Console.WriteLine("Undo — 'dotnet' etiketi geri alınıyor...");
PrintResult(editor.Undo());

Console.WriteLine();

Console.WriteLine("Undo — 'design-patterns' etiketi geri alınıyor...");
PrintResult(editor.Undo());

Console.WriteLine();

Console.WriteLine("Undo — 'csharp' etiketi geri alınıyor...");
PrintResult(editor.Undo());

Console.WriteLine();

// Redo zinciri
Console.WriteLine(" REDO ZİNCİRİ");
Console.WriteLine();

Console.WriteLine(" Redo — 'csharp' etiketi geri getiriliyor...");
PrintResult(editor.Redo());

Console.WriteLine();

Console.WriteLine("Redo — 'design-patterns' etiketi geri getiriliyor...");
PrintResult(editor.Redo());

Console.WriteLine();

Console.WriteLine(" YENİ DEĞİŞİKLİK -> REDO STACK TEMİZLENİR");
Console.WriteLine();

Console.WriteLine(" Yeni tag 'gof' ekleniyor (Redo stack temizlenecek)...");
PrintResult(editor.AddTag("gof"));

Console.WriteLine();

Console.WriteLine(" Redo deneniyor (stack boş olmalı)...");
PrintResult(editor.Redo());

Console.WriteLine();

// Undo limit testi
Console.WriteLine("   UNDO LİMİT TESTİ");
Console.WriteLine();

Console.WriteLine(" Tüm değişiklikler geri alınıyor...");
while (editor.GetCurrentState().CanUndo)
{
    var undoResult = editor.Undo();
    Console.WriteLine($" -> {undoResult.Message}");
}

Console.WriteLine();
Console.WriteLine(" Undo stack boşken Undo deneniyor...");
PrintResult(editor.Undo());

Console.WriteLine();
Console.WriteLine("══════════════════════════════════════════════════════════════");
Console.WriteLine();
Console.WriteLine(" Memento Pattern ile:");
Console.WriteLine("  Document yalnızca kendi state'ini yönetiyor (SRP)");
Console.WriteLine("  DocumentHistory undo/redo yığınlarını yönetiyor");
Console.WriteLine("  Deep copy ile snapshot'lar birbirinden izole");
Console.WriteLine("  Redo stack yeni değişiklikte otomatik temizleniyor");
Console.WriteLine("  DocumentMemento'nun State'i dışarıya kapalı (encapsulation)");

#endregion

// Yardımcı metot — sonuçları formatlı yazdırır
static void PrintResult(DocumentResult result)
{
    if (result.IsSuccess)
    {
        Console.WriteLine($" {result.Message}");
        Console.WriteLine($"   Başlık   : {result.Title}");
        Console.WriteLine($"   İçerik   : {(result.Content?.Length > 40
            ? result.Content[..40] + "..."
            : result.Content)}");
        Console.WriteLine($"  Etiketler: [{string.Join(", ", result.Tags ?? [])}]");
        Console.WriteLine($"  Undo     : {(result.CanUndo ? "var" : "yok")} " +
                          $"| Redo: {(result.CanRedo ? " var" : " yok")}");
    }
    else
    {
        Console.WriteLine($" {result.Message}");
    }
}
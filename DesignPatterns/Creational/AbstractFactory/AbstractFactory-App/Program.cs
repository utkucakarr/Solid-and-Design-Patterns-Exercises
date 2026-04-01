using AbstractFactory_Implementetion.Application;
using AbstractFactory_Implementetion.DarkTheme;
using AbstractFactory_Implementetion.LightTheme;
using AbstractFactory_Violation;
using static System.Net.Mime.MediaTypeNames;

Console.WriteLine("╔══════════════════════════════════════════╗");
Console.WriteLine("║  ABSTRACT FACTORY İHLALİ — CANLI DEMO    ║");
Console.WriteLine("╚══════════════════════════════════════════╝\n");

var badScreen = new UIScreenBad();

Console.WriteLine("─── Tutarlı UI (Şans eseri) ───\n");
badScreen.Render("light", "light", "light");

Console.WriteLine();
Console.WriteLine("─── Tutarsız UI (Karışık tema) ───\n");

// Hiçbir şey bunu engelleyemiyor!
badScreen.Render("light", "dark", "light");

Console.WriteLine();
Console.WriteLine("  Sorunlar:");
Console.WriteLine("  -> Farklı temalar karıştırılabiliyor — tutarsız UI!");
Console.WriteLine("  -> Compiler bunu engelleyemiyor");
Console.WriteLine("  -> Yeni tema = her bileşen için ayrı if-else\n");

Console.WriteLine("╔══════════════════════════════════════════╗");
Console.WriteLine("║     ABSTRACT FACTORY ÇÖZÜMÜ               ║");
Console.WriteLine("╚══════════════════════════════════════════╝");

// Light tema — tüm bileşenler otomatik uyumlu
var lightApp = new UIApplication(new LightUIFactory());
lightApp.RenderUI();
lightApp.SimulateInteraction();

Console.WriteLine();

// Dark tema — tüm bileşenler otomatik uyumlu
var darkApp = new UIApplication(new DarkUIFactory());
darkApp.RenderUI();
darkApp.SimulateInteraction();

Console.WriteLine();
Console.WriteLine("  Yeni tema (HighContrast) eklemek istesek:");
Console.WriteLine("  -> HighContrastButton, HighContrastTextBox, HighContrastCheckBox");
Console.WriteLine("  -> HighContrastUIFactory sınıfları oluşturulur");
Console.WriteLine("  -> UIApplication'a hiç dokunulmaz!\n");

Console.WriteLine("─── SONUÇ ───");
Console.WriteLine("  Bad  -> Temalar karıştırılabilir — tutarsız UI riski");
Console.WriteLine("  Good -> Factory uyumlu aile garantisi — tutarlı UI\n");
using Builder_Implementation.Builders;
using Builder_Implementation.Director;
using Builder_Violation.Computers;

Console.WriteLine("╔══════════════════════════════════════╗");
Console.WriteLine("║   BUILDER İHLALİ — CANLI DEMO         ║");
Console.WriteLine("╚══════════════════════════════════════╝\n");

Console.WriteLine("--- Telescoping Constructor ile Bilgisayar Oluşturma ---\n");

// Hangi bool ne anlama geliyor?
// Sıra değişirse compiler hata vermez ama yanlış çalışır!
var badComputer = new ComputerBad(
    "Intel i9",   // cpu
    32,            // ramGB
    1000,          // storageGB
    "RTX 4090",   // gpu
    "Windows 11", // os
    true,          // hasWifi       <- hangi bool bu?
    false,         // hasBluetooth  <- hangi bool bu?
    "Liquid",      // coolingSystem
    "850W",        // powerSupply
    "ASUS ROG"    // motherboard
);

Console.WriteLine($"  CPU: {badComputer.CPU}");
Console.WriteLine($"  RAM: {badComputer.RamGB} GB");
Console.WriteLine();
Console.WriteLine("  Sorunlar:");
Console.WriteLine("  -> 10 parametre — hangi bool neyi temsil ediyor?");
Console.WriteLine("  -> true/false sırası değişirse compiler hata vermez!");
Console.WriteLine("  -> Yeni özellik eklemek tüm çağrıları bozmak demek\n");

Console.WriteLine("╔══════════════════════════════════════╗");
Console.WriteLine("║   BUILDER ÇÖZÜMÜ — DOĞRU YAKLAŞIM    ║");
Console.WriteLine("╚══════════════════════════════════════╝\n");

// Director ile standart konfigürasyonlar
var gamingBuilder = new GamingComputerBuilder();
var director = new ComputerDirector(gamingBuilder);

Console.WriteLine("--- Gaming Bilgisayar (Director ile) ---\n");
var gamingComputer = director.BuildGamingComputer();
Console.WriteLine(gamingComputer);

Console.WriteLine("--- Ofis Bilgisayar (Director ile) ---\n");
director.ChangeBuilder(new OfficeComputerBuilder());
var officeComputer = director.BuildOfficeComputer();
Console.WriteLine(officeComputer);

Console.WriteLine("--- Sunucu (Director ile) ---\n");
director.ChangeBuilder(new ServerComputerBuilder());
var serverComputer = director.BuildServerComputer();
Console.WriteLine(serverComputer);

// Director olmadan — method chaining ile özel konfigürasyon
Console.WriteLine("--- Özel Konfigürasyon (Method Chaining) ---\n");

var customComputer = new GamingComputerBuilder()
    .SetCPU("AMD Ryzen 9 7950X")
    .SetRAM(64)
    .SetStorage(4000)
    .SetGPU("AMD RX 7900 XTX")
    .SetOS("Windows 11 Pro")
    .SetWifi(true)
    .SetBluetooth(false)
    .SetCoolingSystem("Liquid Cooling 240mm")
    .SetPowerSupply("1200W 80+ Platinum")
    .SetMotherboard("MSI MEG X670E ACE")
    .Build();

Console.WriteLine(customComputer);

Console.WriteLine("--- SONUÇ ---");
Console.WriteLine("  Bad  -> Telescoping constructor — okunaksız, hataya açık");
Console.WriteLine("  Good -> Builder + Director — okunabilir, güvenli, esnek\n");

using Command_Implementation.Commands;
using Command_Implementation.Devices;
using Command_Implementation.Extensions;
using Command_Implementation.Interfaces;
using Command_Implementation.Invoker;
using Command_Violation;
using Microsoft.Extensions.DependencyInjection;

Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
Console.WriteLine("║  ❌ VIOLATION — Command Pattern olmadan                      ║");
Console.WriteLine("╚══════════════════════════════════════════════════════════════╝\n");

var bad = new SmartHomeControllerBad();

Console.WriteLine(bad.TurnOnLivingRoomLight());
Console.WriteLine(bad.SetTemperature(22));
Console.WriteLine(bad.ArmCamera());

Console.WriteLine("\n  Undo yapmak istiyoruz...");
Console.WriteLine("   Undo metodu yok! Hiçbir işlem geri alınamaz.\n");

Console.WriteLine("  İyi Geceler Modu (hardcoded makro):");
bad.GoodNightMode().ForEach(r => Console.WriteLine($"   {r}"));

Console.WriteLine("\n Balkon ışığı eklemek istiyoruz...");
Console.WriteLine("   GoodNightMode() metodunu açıp içine yeni satır eklememiz gerekiyor! (OCP ihlali)\n");

Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
Console.WriteLine("║  ✅ IMPLEMENTATION — Command Pattern ile                     ║");
Console.WriteLine("╚══════════════════════════════════════════════════════════════╝\n");

// DI Container — cihazlar Singleton (fiziksel cihaz tek instance)
var services = new ServiceCollection();

services.AddSmartHomeServices();

var provider = services.BuildServiceProvider();

var controller = provider.GetRequiredService<SmartHomeController>();
var light = provider.GetRequiredService<Light>();
var thermostat = provider.GetRequiredService<Thermostat>();
var camera = provider.GetRequiredService<SecurityCamera>();

// Execute
Console.WriteLine("Komutlar çalıştırılıyor...");
var r1 = controller.Execute(new TurnOnLightCommand(light));
Console.WriteLine($"   {r1.Message}");
var r2 = controller.Execute(new SetTemperatureCommand(thermostat, 25));
Console.WriteLine($"   {r2.Message}");
var r3 = controller.Execute(new ArmCameraCommand(camera));
Console.WriteLine($"   {r3.Message}");

Console.WriteLine($"\n Durum -> Işık: {light.IsOn} | Sıcaklık: {thermostat.Temperature}°C | Kamera: {camera.IsArmed}");

// Undo
Console.WriteLine("\n Undo (sırayla geri alınıyor)...");
Console.WriteLine($"   {controller.Undo().Message}");  // camera undo
Console.WriteLine($"   {controller.Undo().Message}");  // temp undo
Console.WriteLine($"   {controller.Undo().Message}");  // light undo

Console.WriteLine($"\n Durum -> Işık: {light.IsOn} | Sıcaklık: {thermostat.Temperature}°C | Kamera: {camera.IsArmed}");

// Redo
Console.WriteLine("\n Redo (son geri alınan yeniden yapılıyor)...");
Console.WriteLine($"   {controller.Redo().Message}");  // light redo

// Macro Command
Console.WriteLine("\n İyi Geceler Makro Komutu...");
var bedroomLight = new Light("Yatak Odası");   // yeni cihaz — controller'a dokunmadan eklendi!
var goodNight = new MacroCommand("İyi Geceler", new List<ICommand>
{
    new TurnOffLightCommand(light),
    new TurnOffLightCommand(bedroomLight),
    new ArmCameraCommand(camera)
});


var macroResult = controller.Execute(goodNight);
Console.WriteLine($"   {macroResult.Message}");
Console.WriteLine($"\n Durum -> Oturma: {light.IsOn} | Yatak: {bedroomLight.IsOn} | Kamera: {camera.IsArmed}");

// Macro Undo
Console.WriteLine("\n Makro Undo (ters sırayla geri alınıyor)...");
Console.WriteLine($"   {controller.Undo().Message}");
Console.WriteLine($"\n Durum -> Oturma: {light.IsOn} | Yatak: {bedroomLight.IsOn} | Kamera: {camera.IsArmed}");

// Queue
Console.WriteLine("\nKomut Kuyruğu (sabah rutini)...");
controller.Enqueue(new TurnOffLightCommand(light));
controller.Enqueue(new SetTemperatureCommand(thermostat, 21));
controller.RunQueue().ToList().ForEach(r => Console.WriteLine($"   {r.Message}"));

// Log
Console.WriteLine("\n Komut Geçmişi (tam log):");
controller.GetLog().ToList().ForEach(l => Console.WriteLine($"   {l}"));

Console.WriteLine("\n Yeni cihaz eklemek için SmartHomeController'a hiç dokunmadık!");
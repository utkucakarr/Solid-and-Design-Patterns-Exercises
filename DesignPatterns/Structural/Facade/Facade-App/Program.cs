using Facade_Implementation.Configurations;
using Facade_Implementation.Facade;
using Facade_Violation;
using Microsoft.Extensions.DependencyInjection;

Console.WriteLine("╔══════════════════════════════════════╗");
Console.WriteLine("║   FACADE İHLALİ — CANLI DEMO          ║");
Console.WriteLine("╚══════════════════════════════════════╝\n");

var badClient = new BankingClientBad();

Console.WriteLine("--- Transfer Yapılıyor (Client tüm adımları yönetiyor) ---\n");
badClient.Transfer("ACC_001", "ACC_002", 2500);

Console.WriteLine();
Console.WriteLine("  Sorunlar:");
Console.WriteLine("  -> Client 5 alt sistemi biliyor — yüksek bağımlılık");
Console.WriteLine("  -> Adım sırası client'ta — iş mantığı sızdı");
Console.WriteLine("  -> Rollback yok — bakiye düştü ama transfer patlarsa?");
Console.WriteLine("  -> Limit kontrolü eklemek = tüm client'lara dokun\n");

Console.WriteLine("╔══════════════════════════════════════╗");
Console.WriteLine("║   FACADE ÇÖZÜMÜ — DOĞRU YAKLAŞIM     ║");
Console.WriteLine("╚══════════════════════════════════════╝\n");

// Dependency injection Container'ı oluşturulması
var services = new ServiceCollection();

// 2. Bağımlılıkların çağırılması
services.AddBankingServices();

// 3. Provider'ı inşa edip face isteme
var serviceProvider = services.BuildServiceProvider();
var facade = serviceProvider.GetRequiredService<BankingFacade>();

Console.WriteLine("--- Transfer Yapılıyor (Client sadece Transfer() biliyor) ---\n");

var result = facade.Transfer("ACC_001", "ACC_002", 2500);

Console.WriteLine();
Console.WriteLine(result.IsSuccess
    ? $" Success: {result.Message}"
    : $" Fail: {result.Message}");

Console.WriteLine();
Console.WriteLine("  Döviz kuru kontrolü eklemek istesek:");
Console.WriteLine("  -> IExchangeService + ExchangeService — sadece bu eklenir");
Console.WriteLine("  -> BankingFacade güncellenir — client'a hiç dokunulmaz!\n");

Console.WriteLine("--- SONUÇ ---");
Console.WriteLine("  Fail -> Client 5 alt sistemi biliyor — sıkı bağımlılık");
Console.WriteLine("  Success -> Facade ile tek metod — alt sistemler tamamen gizli\n");
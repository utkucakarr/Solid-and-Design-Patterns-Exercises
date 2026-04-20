# Command Pattern 

> *"Bir isteği bir nesne olarak kapsülleyin; böylece istemcileri farklı isteklerle parametrelendirebilir, istekleri kuyruğa alabilir veya loglayabilir ve geri alınabilir işlemleri destekleyebilirsiniz."*
> — **Gang of Four**

---

## Command Pattern Nedir?

**Command Pattern**, bir isteği (request) nesne olarak kapsülleyen davranışsal bir tasarım kalıbıdır. Gönderen (invoker) ile alıcı (receiver) arasındaki bağı koparır; her işlemi bağımsız bir nesneye dönüştürür.

### Ne Zaman Kullanılır?

- İşlemlerin **geri alınabilmesi (undo)** veya **yeniden yapılabilmesi (redo)** gerektiğinde
- İşlemlerin **kuyruğa alınması** veya **zamanlanması** gerektiğinde
- **Makro komutlar** (birden fazla işlemi tek komutla tetiklemek) kurgulanacaksa
- **İşlem geçmişi (audit log)** tutulması gerektiğinde
- Gönderenin alıcıdan **tamamen bağımsız** olması istendiğinde

### Gerçek Hayat Örnekleri

| Örnek | Command Karşılığı |
|---|---|
| Metin editöründe Ctrl+Z | `UndoCommand` |
| Akıllı ev asistanı "İyi geceler" | `MacroCommand` |
| CI/CD pipeline job'ları | `DeployCommand`, `RollbackCommand` |
| Restoran sipariş sistemi | `OrderCommand`, `CancelCommand` |
| Veritabanı transaction | `CommitCommand`, `RollbackCommand` |

---

## Senaryo
## Akıllı Ev Otomasyon Sistemi

Kullanıcı ışıkları, termostatı ve güvenlik kamerasını kontrol ediyor. Her komut kaydediliyor, geri alınabiliyor (undo), makrolar oluşturulabiliyor ("İyi geceler" diyince tüm ışıklar söner, alarm kurulur).
Neden iyi?
- Undo/Redo Command'in en güçlü özelliğini doğal gösteriyor
- Macro Command (composite) eklenebilir
- Queue/Scheduler'a takılabilir
- Gerçek IoT projelerinde tam bu şekilde kullanılıyor

---

## Kötü Kullanım

```csharp
public class SmartHomeController_Bad
{
    // Tüm cihazlar doğrudan controller'a bağımlı
    private readonly Light_Bad _livingRoomLight;
    private readonly Thermostat_Bad _thermostat;
    private readonly SecurityCamera_Bad _camera;

    // Geçmiş tutulmuyor — undo/redo tamamen imkânsız

    public string TurnOnLivingRoomLight()
    {
        _livingRoomLight.TurnOn();
        return "Işık açıldı."; // Önceki durum hiç kaydedilmedi
    }

    public string SetTemperature(int temperature)
    {
        _thermostat.SetTemperature(temperature); // Önceki sıcaklık kayboldu
        return $"Sıcaklık {temperature}°C";
    }

    // Makro hardcoded — yeni cihaz eklenince bu metot değişmek zorunda
    public List<string> GoodNightMode()
    {
        _livingRoomLight.TurnOff();
        _camera.Arm();
        // Bu işlemleri tek seferde geri almak imkânsız
        return new List<string> { "Işık söndürüldü.", "Kamera aktif." };
    }
}
```

### Sonuçlar

| Sorun | Etki |
|---|---|
| Undo/Redo yok | Kullanıcı hatayı geri alamaz |
| Cihazlar hardcoded | Yeni cihaz = controller değişimi (OCP ihlali) |
| Makro hardcoded | Yeni komut = metot içini açmak gerekir |
| Geçmiş yok | Audit log tutmak imkânsız |
| Kuyruk yok | Zamanlı görev desteği sıfır |

---

## Doğru Kullanım

```csharp
// ICommand — tüm komutların kontratı
public interface ICommand
{
    string Description { get; }
    CommandResult Execute();
    CommandResult Undo();
}

// Concrete Command — önceki değeri saklar
public class SetTemperatureCommand : ICommand
{
    private int _previousTemperature;

    public CommandResult Execute()
    {
        _previousTemperature = _thermostat.SetTemperature(_newTemperature); // kaydedildi
        return CommandResult.Success(...);
    }

    public CommandResult Undo()
    {
        _thermostat.SetTemperature(_previousTemperature); // tam geri dönüş
        return CommandResult.Success(...);
    }
}

// Invoker — ICommand'dan başka hiçbir şey bilmez
public class SmartHomeController
{
    private readonly Stack<ICommand> _history   = new();
    private readonly Stack<ICommand> _redoStack = new();
    private readonly Queue<ICommand> _queue     = new();

    public CommandResult Execute(ICommand command)
    {
        var result = command.Execute();
        if (result.IsSuccess)
        {
            _history.Push(command);
            _redoStack.Clear();
        }
        return result;
    }

    public CommandResult Undo()
    {
        if (_history.Count == 0)
            return CommandResult.Fail("Geri alınacak komut bulunamadı.", ...);

        var command = _history.Pop();
        var result  = command.Undo();
        if (result.IsSuccess) _redoStack.Push(command);
        return result;
    }
}

// MacroCommand — undo ters sırada
public class MacroCommand : ICommand
{
    public CommandResult Undo()
    {
        foreach (var command in _commands.Reverse()) // ters sıra
            command.Undo();
        return CommandResult.Success(...);
    }
}
```

---

## Pattern'in Yaptığı İşlem

| Adım | Aktör | Açıklama |
|---|---|---|
| 1 | Client | Concrete Command oluşturur, Receiver'ı enjekte eder |
| 2 | Client | Komutu Invoker'a (SmartHomeController) verir |
| 3 | Invoker | `command.Execute()` çağırır |
| 4 | Command | Receiver metodunu çağırır, önceki durumu saklar |
| 5 | Invoker | Komutu `_history` stack'ine ekler |
| 6 | Client | `Undo()` isterse Invoker stack'ten komut alır |
| 7 | Command | Önceki durumu geri yükler |

---

## Farkın Özeti

| Özellik | Bad | Good |
|---|---|---|
| Undo / Redo | Yok | Stack tabanlı, tam destek |
| Yeni komut ekleme | Controller değişir | Yeni class, hiçbir şey değişmez |
| Makro komut | Hardcoded metot | `MacroCommand` + ters undo |
| Komut kuyruğu | Yok | `Queue<ICommand>` |
| Audit log | Yok | Her execute/undo loglanır |
| Bağımlılık | Controller cihazları bilir | Controller yalnızca `ICommand` bilir |
| OCP | İhlal | Yeni komut = yeni class |

---

## Testler

### Neden Moq Kullanılmadı?

Command Pattern testlerinde **gerçek cihazlar (Light, Thermostat, SecurityCamera)** son derece hafif ve yan etkisizdir. Undo/Redo doğruluğunu kanıtlamak için cihazın **gerçek state değişikliği** gözlemlenmesi gerekir; mock nesne bu geri dönüşü simüle edemez. Bu yüzden Moq yerine gerçek nesneler kullanıldı.

### Kapsanan Senaryolar

| Kategori | Test Sayısı |
|---|---|
| Happy path (execute) | 4 |
| Undo / Redo | 6 |
| Macro Command | 2 |
| Queue | 2 |
| Başarısız senaryolar | 2 |
| Log doğrulama | 2 |
| History count | 2 |
| Guard clause | 4 |
| Constructor null guard | 8 |
| Result model | 4 |
| **Toplam** | **36** |

---

## SOLID ile Bağlantısı

| Prensip | Bağlantı |
|---|---|
| **S** — Single Responsibility | Her komut yalnızca bir işi yapar |
| **O** — Open/Closed | Yeni komut = yeni class, invoker değişmez |
| **L** — Liskov Substitution | Tüm komutlar `ICommand` yerine geçebilir |
| **I** — Interface Segregation | `ICommand` yalnızca `Execute` ve `Undo` içerir |
| **D** — Dependency Inversion | Invoker somut sınıfa değil `ICommand`'a bağlıdır |

---

## Diğer Pattern'lerle İlişkisi

| Pattern | İlişki |
|---|---|
| **Composite** | `MacroCommand` doğrudan Composite kullanır — komutları ağaç yapısında birleştirir |
| **Memento** | Command undo için state saklar; karmaşık state varsa Memento ile birleştirilir |
| **Chain of Responsibility** | İkisi de isteği nesneleştirir; CoR zincirden geçirir, Command kuyruğa/stack'e ekler |
| **Strategy** | Command gibi davranışı kapsüller; fark: Command geçmiş/undo yönetir, Strategy yönetmez |
| **Facade (8)** | Facade subsystem'ları gizler; Command işlemleri kapsüller — birlikte kullanıldığında makro komutlar facade üzerinden çalışır |

---

## Kullanılan Teknolojiler

| Teknoloji | Versiyon | Kullanım Amacı |
|---|---|---|
| C# / .NET | 8.0 | Dil ve platform |
| Microsoft.Extensions.DependencyInjection | 10.0.6 | DI container (App projesi) |
| xUnit | 2.5.3 | Test framework |
| FluentAssertions | 8.9.0 | Okunabilir assertion'lar |

---

## Design Patterns Serisi

| # | Pattern | Kategori | Durum |
|---|---|---|---|
| 1 | Strategy | Behavioral | ✅ Tamamlandı |
| 2 | Command | Behavioral | ✅ Tamamlandı |
| 3 | Iterator | Behavioral | 🔜 Yakında |
| 4 | Template Metot | Behavioral | 🔜 Yakında |
| 5 | Observer | Behavioral | 🔜 Yakında |
| 6 | Memento | Behavioral | 🔜 Yakında |
| 7 | Mediator | Behavioral | 🔜 Yakında |
| 8 | Chain Of Responsibility | Behavioral | 🔜 Yakında |
| 9 | Visitor | Behavioral | 🔜 Yakında |
| 10 | State | Behavioral | 🔜 Yakında |

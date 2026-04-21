# Mediator Pattern — Gerçek Zamanlı Chat Odası Sistemi

> *"Define an object that encapsulates how a set of objects interact. Mediator promotes loose coupling by keeping objects from referring to each other explicitly, and it lets you vary their interaction independently."*
> *"Bir nesne kümesinin nasıl etkileşime girdiğini kapsülleyen bir nesne tanımlar. Arabulucu (Mediator), nesnelerin birbirine açıkça referans vermesini engelleyerek gevşek bağlılığı (loose coupling) teşvik eder ve etkileşimlerini bağımsız olarak değiştirmenize olanak tanır."*
> — **Gang of Four, Design Patterns**

---

## Pattern Nedir?

**Mediator Pattern**, bir grup nesnenin birbirleriyle doğrudan iletişim kurması yerine merkezi bir koordinatör (Mediator) üzerinden haberleştiği davranışsal bir tasarım kalıbıdır. Nesneler birbirini tanımaz; yalnızca Mediator'ı bilir. Bu sayede N*(N-1) many-to-many bağımlılık, N adet one-to-one bağımlılığa indirgenir.

### Ne Zaman Kullanılır?

- Nesneler arasında karmaşık many-to-many iletişim varsa
- Bir nesnenin değişmesi çok sayıda başka nesneyi etkiliyorsa
- Nesnelerin birbirini doğrudan referans tutması yeniden kullanımı zorlaştırıyorsa
- İletişim mantığını (yetki kontrolü, yönlendirme) tek noktada toplamak gerekiyorsa
- Loose coupling ile birbirinden bağımsız geliştirilebilen bileşenler isteniyorsa

### Gerçek Hayat Örnekleri

| Domain | Mediator | Colleague'lar |
|---|---|---|
| Chat Uygulaması | ChatRoom | RegularUser, AdminUser, BotUser |
| Hava Trafik Kontrolü | Kule | Uçaklar |
| UI Framework | EventBus | Button, TextBox, Form |
| Mikroservis | API Gateway | Auth, Order, Payment servisleri |
| CI/CD | Pipeline Orchestrator | Build, Test, Deploy adımları |

---

## Senaryo

> Bir gerçek zamanlı chat uygulaması geliştiriyorsunuz. Kullanıcılar (Regular, Admin, Bot) birbirleriyle mesajlaşabiliyor, admin broadcast ve kick yapabiliyor, bot otomatik yanıt veriyor. Yeni kullanıcı tipleri kolayca eklenebilmeli. Sistemi nasıl tasarlarsın?

---

## Kötü Kullanım (Violation)

```csharp
public class RegularUser_Bad
{
    // Her kullanıcı diğerlerini doğrudan referans tutuyor
    // N kullanıcı için N*(N-1) bağımlılık oluşuyor
    private readonly List<RegularUser_Bad> _contacts = new();
    private readonly List<AdminUser_Bad> _adminContacts = new();
    private readonly List<BotUser_Bad> _botContacts = new();

    public void SendMessage(string content)
    {
        // Her tip için ayrı iterate — yeni tip eklemek bu metodu şişiriyor
        foreach (var contact in _contacts)
            contact.ReceiveMessage(message);

        foreach (var admin in _adminContacts)
            admin.ReceiveMessage(message); // DRY ihlali

        foreach (var bot in _botContacts)
            bot.ReceiveMessage(message);   // OCP ihlali
    }
}

public class BotUser_Bad
{
    // Bot yalnızca RegularUser listesini tutuyor
    // AdminUser'dan mesaj gelirse yanıt verilemiyor
    private readonly List<RegularUser_Bad> _regularUsers = new();
}
```

### Violation Sonuçları

| Sorun | Açıklama |
|---|---|
| Many-to-Many Bağımlılık | 4 kullanıcı için 12 bağımlılık — N arttıkça katlanarak büyür |
| OCP İhlali | Yeni kullanıcı tipi eklemek tüm sınıfların güncellenmesini gerektirir |
| DRY İhlali | Her sınıfta aynı iterate mantığı tekrar ediyor |
| Kısıtlı İletişim | Bot yalnızca RegularUser'a yanıt verebiliyor |
| Yetki Kontrolü Dağınık | Her sınıf kendi yetkisini kendisi yönetmeli |
| Test Edilemezlik | Gerçek nesneler birbirine sıkı bağlı — mock'lanamaz |

---

## Doğru Kullanım (Implementation)

```csharp
// Mediator — tüm iletişimi koordine eder
public class ChatRoom : IChatMediator
{
    private readonly Dictionary<string, IUser> _users = new();

    public void Register(IUser user)
    {
        // Colleague sadece Mediator'ı tanır
        user.SetMediator(this);
        _users[user.Username] = user;
    }

    public ChatResult SendMessage(string senderUsername, string content)
    {
        // Yönlendirme tek noktada — Colleague'lar birbirini bilmez
        var deliveredCount = NotifyAll(
            ChatMessage.Public(senderUsername, content),
            excludeUsername: senderUsername);

        return ChatResult.Success("Mesaj iletildi.", senderUsername,
            deliveredCount, MessageType.Public);
    }

    public ChatResult Broadcast(string senderUsername, string content)
    {
        // Yetki kontrolü Mediator'da — Colleague bilmez
        if (_users[senderUsername].Role != UserRole.Admin)
            return ChatResult.Fail("Broadcast yetkisi yok.");

        var deliveredCount = NotifyAll(
            ChatMessage.Broadcast(senderUsername, content),
            excludeUsername: null);

        return ChatResult.Success("Broadcast iletildi.", senderUsername,
            deliveredCount, MessageType.Broadcast);
    }
}

// Colleague — yalnızca Mediator'ı bilir
public class RegularUser : IRegularUser
{
    private IChatMediator? _mediator;

    public ChatResult SendMessage(string content)
    {
        // Diğer kullanıcıları tanımıyor — Mediator üzerinden iletişim
        return _mediator!.SendMessage(Username, content);
    }
}
```

---

## Pattern'in Yaptığı İşlem

| Adım | Aktör | Eylem |
|---|---|---|
| 1 | `Program.cs` | `ChatRoom.Register(user)` ile kullanıcıları kaydeder |
| 2 | `ChatRoom` | `user.SetMediator(this)` ile her Colleague'a kendini tanıtır |
| 3 | `RegularUser` | `_mediator.SendMessage(Username, content)` çağırır |
| 4 | `ChatRoom` | Göndereni doğrular, mesajı oluşturur |
| 5 | `ChatRoom` | `NotifyAll` ile gönderen hariç tüm kullanıcılara iletir |
| 6 | Her `IUser` | `ReceiveMessage(message)` ile mesajı alır ve işler |
| 7 | `BotUser` | Public mesaj alınca `_mediator.SendPrivateMessage` ile yanıt verir |
| 8 | `ChatRoom` | Yetki kontrollerini (Broadcast, Kick) merkezi olarak uygular |

---

## Farkın Özeti

| Özellik | Bad | Good |
|---|---|---|
| Bağımlılık sayısı | N*(N-1) — katlanarak büyür | N — lineer büyür |
| Yeni tip ekleme | Tüm sınıflar değiştirilmeli | Sadece yeni sınıf + Register |
| Yetki kontrolü | Dağınık — her sınıfta ayrı | Merkezi — yalnızca ChatRoom'da |
| Bot kısıtı | Yalnızca RegularUser'a yanıt | Tüm kullanıcı tiplerine yanıt |
| Test edilebilirlik | Gerçek nesneler sıkı bağlı | Mock IChatMediator ile izole test |
| OCP | İhlal | Korunuyor |
| DRY | İhlal | Korunuyor |

---

## Testler

### Neden Moq?

Mediator Pattern'de Colleague sınıfları `IChatMediator` arayüzüne bağımlıdır. Moq ile:
- `IChatMediator` mock'lanarak Colleague'ların doğru metodları çağırıp çağırmadığı doğrulanır
- `IUser` mock'lanarak `ChatRoom`'un doğru kullanıcılara mesaj iletip iletmediği test edilir
- `Times.Once`, `Times.Never` ile çağrı sayısı doğrulanır
- Gerçek ChatRoom olmadan her Colleague bağımsız test edilebilir

### Kapsanan Senaryolar

| Kategori | Test Sayısı | Örnekler |
|---|---|---|
| Register/Unregister | 5 | Kayıt, mükerrer kayıt, ayrılma, aktif kullanıcı listesi |
| Public Mesaj | 5 | Başarılı gönderim, iletim sayısı, gönderene iletilmeme |
| Özel Mesaj | 4 | Başarılı gönderim, yalnızca alıcıya iletim, bulunamayan alıcı |
| Broadcast | 4 | Admin başarılı, regular yetkisiz, tüm kullanıcılara iletim |
| Kick | 5 | Admin başarılı, regular yetkisiz, hedef yok, self-kick, bildirim |
| RegularUser Delegate | 5 | SendMessage, SendPrivate, LeaveRoom, ReceiveMessage, no mediator |
| AdminUser Delegate | 3 | Broadcast, KickUser, LeaveRoom delegasyonu |
| BotUser | 3 | Public yanıt, private'a yanıt yok, kendi mesajına yanıt yok |
| Guard Clause | 7 | Boş content, whitespace username, null user |
| Constructor Null Guard | 8 | ChatRoom, RegularUser, AdminUser, BotUser, SetMediator |

---

## SOLID ile Bağlantısı

| Prensip | Bağlantı |
|---|---|
| **S**RP | `ChatRoom` iletişimi koordine eder; her Colleague yalnızca kendi davranışını bilir |
| **O**CP | Yeni kullanıcı tipi eklemek mevcut kodu değiştirmez — sadece `IUser` implemente et ve `Register` et |
| **L**SP | Tüm `IUser` implementasyonları `ChatRoom`'da birbirinin yerine geçebilir |
| **I**SP | `IRegularUser`, `IAdminUser`, `IBotUser` ayrı arayüzlerde — her tip yalnızca ihtiyacı olanı implemente eder |
| **D**IP | `ChatRoom` somut sınıflara değil `IUser` arayüzüne; Colleague'lar `IChatMediator` arayüzüne bağımlı |

---

## Diğer Pattern'lerle İlişkisi

| Pattern | İlişki |
|---|---|
| **Observer (9. Pattern)** | Observer'da Subject → Observer tek yönlü yayın yapar; Subject Observer'ları listeler. Mediator'da iletişim çift yönlüdür ve nesneler birbirini hiç bilmez — tüm mesajlar Mediator üzerinden akar. Chat odası için Mediator daha uygun; bildirim sistemi için Observer. |
| **Facade (8. Pattern)** | Facade karmaşık bir alt sistemi basit bir arayüzle saklar — tek yönlü, dışarıdan içeriye. Mediator ise nesneler arası iletişimi koordine eder — çift yönlü, içten içe. Facade kullanıcısı alt sistemi bilmez; Mediator Colleague'lar birbirini bilmez. |
| **Command** | Mediator ile Command birlikte kullanılabilir: her mesaj gönderme isteği bir Command nesnesi olarak sarılabilir, böylece undo/redo ve mesaj kuyruğu desteklenir. |
| **Proxy** | API Gateway senaryosunda Mediator ile Proxy iç içe geçer: Proxy dış istekleri alır, Mediator iç servislere yönlendirir. |

---

## Kullanılan Teknolojiler

| Teknoloji | Versiyon | Kullanım Amacı |
|---|---|---|
| C# / .NET | 8.0 | Ana geliştirme platformu |
| Microsoft.Extensions.DependencyInjection | 10.0.6 | DI container |
| xUnit | 2.53 | Test framework |
| FluentAssertions | 8.9.0 | Okunabilir assertion'lar |
| Moq | 4.20.72 | IChatMediator ve IUser mock'lama |

---

## Design Patterns Serisi

| # | Pattern | Kategori | Durum |
|---|---|---|---|
| 1 | Strategy | Behavioral | ✅ Tamamlandı |
| 2 | Command | Behavioral | ✅ Tamamlandı |
| 3 | Iterator | Behavioral | ✅ Tamamlandı |
| 4 | Template Metot | Behavioral | ✅ Tamamlandı |
| 5 | Observer | Behavioral | ✅ Tamamlandı |
| 6 | Memento | Behavioral | ✅ Tamamlandı |
| 7 | Mediator | Behavioral | ✅ Tamamlandı |
| 8 | Chain Of Responsibility | Behavioral | 🔜 Yakında |
| 9 | Visitor | Behavioral | 🔜 Yakında |
| 10 | State | Behavioral | 🔜 Yakında |
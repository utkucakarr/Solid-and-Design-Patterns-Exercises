# State Pattern — E-Ticaret Sipariş Yönetimi

> *"Allow an object to alter its behavior when its internal state changes. The object will appear to change its class."*
> *"Bir nesnenin iç durumu değiştiğinde davranışını değiştirmesine olanak tanır. Nesne, sanki sınıfını değiştirmiş gibi görünür."*
> — **Gang of Four**

---

## Pattern Nedir?

**State Pattern**, bir nesnenin iç durumu değiştiğinde davranışının da değişmesini sağlayan davranışsal (behavioral) bir tasarım desenidir. Dışarıdan bakıldığında nesne sanki sınıfını değiştirmiş gibi davranır.

### Ne Zaman Kullanılır?

- Bir nesnenin davranışı, içinde bulunduğu **duruma göre farklılaşıyorsa**
- `if/else` veya `switch/case` zincirleri **her yeni durum eklendikçe büyüyorsa**
- **Durum geçiş kuralları** karmaşıklaşıyorsa ve merkezi yönetim gerekiyorsa
- Her durumun **kendi iş kurallarını** kapsüllemesi gerekiyorsa
- Durum sayısı zamanla **artması beklenen** bir domain'de çalışılıyorsa

### Gerçek Hayat Örnekleri

| Domain | State'ler |
|--------|-----------|
| E-Ticaret | Pending → Confirmed → Shipped → Delivered |
| Destek Sistemi | Open → InProgress → Resolved → Closed |
| Belge Onayı | Draft → Review → Approved → Published |
| Trafik Lambası | Red → Green → Yellow |
| ATM | Idle → CardInserted → PinVerified → Dispensing |

---

## Senaryo
> Bir e-ticaret sisteminde sipariş yönetimi modülü geliştiriyoruz.
> Sipariş; Pending, Confirmed, Shipped, Delivered ve Cancelled
> durumlarından geçiyor. Her durumda Confirm, Ship, Deliver ve Cancel
> işlemleri farklı davranıyor. Bazı geçişler geçerli, bazıları yasak.

---

## Kötü Kullanım

```csharp
public class OrderService_Bad
{
    public OrderStatus Status { get; private set; }

    // Her metot tüm state'leri if/switch ile kontrol ediyor
    public string Confirm()
    {
        if (Status == OrderStatus.Pending)
        {
            Status = OrderStatus.Confirmed;
            return "Sipariş onaylandı.";
        }
        else if (Status == OrderStatus.Confirmed)
            return "Zaten onaylanmış.";
        else if (Status == OrderStatus.Shipped)
            return "Kargoda, onaylanamaz.";
        else if (Status == OrderStatus.Delivered)
            return "Teslim edildi, onaylanamaz.";
        else if (Status == OrderStatus.Cancelled)
            return "İptal edildi, onaylanamaz.";

        return "Bilinmeyen durum."; // Asla tetiklenmemeli
    }

    // Ship(), Deliver(), Cancel() metodları da aynı if zincirini tekrarlıyor
    // Yeni bir state eklemek → 4 metodun HEPSİNE if bloğu eklenmesi gerekir
}
```

### Sonuçlar

| Sorun | Açıklama |
|-------|----------|
| OCP İhlali | Yeni state → mevcut metodlar değişmek zorunda |
| Kod Tekrarı | Her metotta aynı if/switch yapısı |
| Gömülü İş Kuralları | "Kargo'dayken iptal edilemez" kuralı nerede? Belirsiz |
| Test Zorluğu | Tüm state kombinasyonları tek sınıfta test edilmek zorunda |
| Ölçeksizlik | 5 state × 4 metod = 20 if bloğu; 6 state olursa 24 olur |

---

## Doğru Kullanım

```csharp
// Her state kendi davranışını kapsüller
public class PendingState : IOrderState
{
    public OrderResult Confirm(OrderContext context)
    {
        var from = GetStatusDescription();
        context.TransitionTo(new ConfirmedState()); // Geçiş burada
        return OrderResult.Success(context.OrderId,
            "Sipariş onaylandı.", from, context.GetStatusDescription());
    }

    public OrderResult Ship(OrderContext context) =>
        OrderResult.Fail(context.OrderId,
            "Onaylanmadan kargoya verilemez.", GetStatusDescription());

    public string GetStatusDescription() => "Ödeme Bekleniyor";
    // ...
}

// Context sadece delege eder — if/switch yok
public class OrderContext : IOrderContext
{
    public IOrderState CurrentState { get; private set; }

    public OrderResult Confirm() => CurrentState.Confirm(this); 
    public OrderResult Ship()    => CurrentState.Ship(this);   
    public OrderResult Deliver() => CurrentState.Deliver(this); 
    public OrderResult Cancel()  => CurrentState.Cancel(this);  

    public void TransitionTo(IOrderState newState) =>
        CurrentState = newState;
}
```

---

## Pattern'in Yaptığı İşlem

| Adım | Açıklama |
|------|----------|
| 1 | `OrderContext` oluşturulur, `PendingState` ile başlar |
| 2 | İstemci `context.Confirm()` çağırır |
| 3 | Context, çağrıyı `CurrentState.Confirm(this)` ile aktif state'e delege eder |
| 4 | `PendingState.Confirm()` geçişin geçerli olduğunu bilir ve `TransitionTo(new ConfirmedState())` çağırır |
| 5 | Context'in `CurrentState`'i artık `ConfirmedState`'tir |
| 6 | Bir sonraki çağrı artık `ConfirmedState` tarafından karşılanır |

---

## Farkın Özeti

| Özellik | Bad (if/switch) | Good (State Pattern) |
|---------|-------------------|------------------------|
| Yeni state ekleme | 4 metodun tamamı değişir | Sadece 1 yeni sınıf eklenir |
| İş kuralı yeri | Metod içine gömülü | İlgili state sınıfında |
| Kod tekrarı | Her metotta if zinciri | Her state bağımsız |
| OCP uyumu | İhlal | Uyumlu |
| Test edilebilirlik | Tüm kombinasyonlar tek yerde | Her state bağımsız test edilir |
| Okunabilirlik | if blokları içinde kaybolur | Her state'in amacı net |
| Geçiş kuralları | Dağınık, belirsiz | State içinde kapsüllendi |

---

## Testler

### Neden Moq Kullanılmadı?

State Pattern'de state sınıfları **somut davranış** içerir; koordinasyon gerçek `OrderContext` üzerinden yürütülür. Mock'lamak yerine gerçek state nesneleri kullanmak hem daha güvenilir hem de pattern'in akışını doğrudan test eder.

### Kapsanan Senaryolar

| Kategori | Test Sayısı | Örnek |
|----------|-------------|-------|
| Happy Path | 6 | Tam yaşam döngüsü, her geçiş |
| Geçersiz Geçişler | 8 | Pending'den teslim, kargodayken iptal |
| Terminal State | 4 | Cancelled state'den her işlem |
| State Geçiş Doğrulama | 4 | FromState / ToState kontrolü |
| Guard Clause | 4 | Boş string, 0, negatif tutar |
| Constructor Null Guard | 5 | Null state, null context |
| Result Nesnesi | 4 | Success/Fail factory metodları |
| Başlangıç State | 2 | PendingState ile başladığını doğrula |

---

## SOLID ile Bağlantısı

| Prensip | Bağlantı |
|---------|----------|
| **S** — Single Responsibility | Her state sınıfı yalnızca kendi durumunun davranışından sorumludur |
| **O** — Open/Closed | Yeni state eklemek mevcut kodu değiştirmez, sadece yeni sınıf eklenir |
| **L** — Liskov Substitution | Tüm state'ler `IOrderState` yerine geçebilir |
| **I** — Interface Segregation | `IOrderState` yalnızca ihtiyaç duyulan metodları içerir |
| **D** — Dependency Inversion | `OrderContext`, somut state'lere değil `IOrderState` arayüzüne bağlıdır |

---

## Diğer Pattern'lerle İlişkisi

| Pattern | İlişki |
|---------|--------|
| **Strategy** | Yapısal olarak benzerdir; fark: Strategy dışarıdan seçilir, State kendi içinde geçiş yapar |
| **Facade** | State Pattern karmaşık geçiş mantığını kapsüllerken, Facade karmaşık alt sistemleri gizler |
| **Adapter** | Adapter farklı arayüzleri uyumlu hale getirir; State ise aynı arayüzü farklı davranışlarla sunar |
| **Factory** | `OrderFactory`, `OrderContext`'i başlangıç state'iyle birlikte oluşturur — State + Factory birlikte sık kullanılır |
| **Command** | Her state aksiyonu bir Command olarak modellenebilir; ikisi birlikte undo/redo destekler |

---

## Kullanılan Teknolojiler

| Teknoloji | Versiyon | Kullanım Amacı |
|-----------|----------|----------------|
| .NET | 8.0 | Platform |
| Microsoft.Extensions.DependencyInjection | 10.0.7 | DI container |
| xUnit | 2.5.3 | Test framework |
| FluentAssertions | 8.9.0 | Okunabilir assertions |

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
| 8 | Chain Of Responsibility | Behavioral | ✅ Tamamlandı |
| 9 | Visitor | Behavioral | ✅ Tamamlandı |
| 10 | State | Behavioral | ✅ Tamamlandı |
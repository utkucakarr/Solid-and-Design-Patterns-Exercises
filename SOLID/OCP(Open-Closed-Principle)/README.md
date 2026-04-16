
# SOLID #2 — Open/Closed Principle (OCP)

> *"Yazılım varlıkları (sınıflar, modüller, fonksiyonlar) genişlemeye açık, değişime kapalı olmalıdır."*
> — Robert C. Martin

---

## Prensip Nedir?

Open/Closed Principle, mevcut ve çalışan bir koda **dokunmadan** yeni davranışlar ekleyebilmemiz gerektiğini söyler.

- **Açık (Open)** → Yeni özellikler eklenebilmeli.
- **Kapalı (Closed)** → Mevcut kod değiştirilmemeli.

Bir sınıfa yeni bir özellik eklemek için içine girip mevcut kodu düzenlemek zorundaysak — o sınıf OCP'yi ihlal ediyor demektir. Her düzenleme, mevcut testleri bozma ve yeni hata ekleme riskini beraberinde getirir.

---

## Kötü Kullanım — OCP İhlali

`DiscountManager` sınıfı içinde `switch-case` kullanılıyor:

```csharp
public decimal CalculateDiscount(decimal amount, CustomerType customerType)
{
    switch (customerType)
    {
        case CustomerType.Standard: return amount * 0.05m;
        case CustomerType.Premium:  return amount * 0.10m;
        case CustomerType.VIP:      return amount * 0.20m;
        default: throw new ArgumentException("Invalid customer type");
    }
}
```

### Yeni bir müşteri tipi (örn: Student) eklemek istesek ne olur?

1. `CustomerType` enum'una `Student` eklenir.
2. `DiscountManager` sınıfının **içine girilir.**
3. Yeni bir `case` eklenir.
4. Tüm mevcut testler yeniden çalıştırılır.
5. Başka `switch-case` kullanan yerler de güncellenir.

Her yeni gereksinim, çalışan kodu değiştirmeyi zorunlu kılar. Bu da mevcut davranışları bozma riskini sürekli canlı tutar.

---

## Doğru Kullanım — OCP Uyumlu Yapı

`IDiscountStrategy` interface'i ile her indirim tipi **bağımsız bir sınıf** olarak tanımlanıyor:

```csharp
public interface IDiscountStrategy
{
    decimal ApplyDiscount(decimal amount);
}
```

```csharp
public class StandartDiscount : IDiscountStrategy
{
    public decimal ApplyDiscount(decimal amount) => amount * 0.05m;
}

public class PremiumDiscount : IDiscountStrategy
{
    public decimal ApplyDiscount(decimal amount) => amount * 0.10m;
}

public class VipDiscount : IDiscountStrategy
{
    public decimal ApplyDiscount(decimal amount) => amount * 0.20m;
}
```

`DiscountCalculator` sınıfı ise **hiç değişmiyor:**

```csharp
public decimal Calculate(decimal amount, IDiscountStrategy strategy)
{
    if (amount <= 0)
        return 0;

    return strategy.ApplyDiscount(amount);
}
```

### Yeni bir müşteri tipi (örn: Student) eklemek istesek ne olur?

```csharp
// Sadece yeni bir sınıf ekliyoruz — başka hiçbir yere dokunmuyoruz!
public class StudentDiscount : IDiscountStrategy
{
    public decimal ApplyDiscount(decimal amount) => amount * 0.15m;
}
```

`DiscountCalculator` değişmez. Mevcut testler bozulmaz. Riske girilmez.

---

## Farkın Özeti

| | OCP İhlali | OCP Uyumlu |
|---|---|---|
| Yeni indirim eklemek | Mevcut sınıfı değiştir | Yeni sınıf ekle |
| Mevcut testler | Bozulabilir | Dokunulmaz |
| Kod değişikliği riski | Her seferinde var | Sıfır |
| Genişletme yöntemi | `switch-case` ekle | Yeni `IDiscountStrategy` |

---

## Testler

Testler **xUnit** ile yazılmıştır.

### Kapsanan Senaryolar

**DiscountCalculatorTests** — Hesaplayıcı davranışı:
- Standart indirimde farklı tutarlar için doğru sonuç `[Theory]`
- Premium indirimde doğru yüzde hesabı `[Fact]`

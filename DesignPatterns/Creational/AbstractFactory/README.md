# Design Patterns #3 — Abstract Factory Pattern

> *"Birbiriyle ilişkili veya bağımlı nesneler ailesini, somut sınıflarını belirtmeden oluşturmak için bir interface sağlar."*
> — Gang of Four (GoF)

---

## Abstract Factory Pattern Nedir?

Abstract Factory Pattern, birbiriyle **uyumlu nesneler ailesini** tek bir çatı altında üretir. Client hangi somut sınıfların kullanıldığını bilmek zorunda kalmaz — sadece factory interface'i üzerinden çalışır.

**Ne zaman kullanılır?**

- Birbiriyle uyumlu olmak zorunda olan nesneler grubu oluşturulacaksa
- Sistem birden fazla ürün ailesiyle çalışacaksa
- Ürün ailesi değiştiğinde tüm bileşenlerin birlikte değişmesi gerekiyorsa

---

## Senaryo
Bir masaüstü uygulamasında 2 farklı tema var:

LightTheme → açık renkli Button, TextBox, CheckBox
DarkTheme → koyu renkli Button, TextBox, CheckBox

Her temada Button, TextBox ve CheckBox birbiriyle uyumlu olmalı — Light Button ile Dark TextBox karıştırılamaz!

---

## Kötü Kullanım — Abstract Factory İhlali

Client her bileşeni ayrı ayrı oluşturuyor — temalar karıştırılabiliyor:

```csharp
public class UIScreen_Bad
{
    public void Render(string buttonTheme, string textBoxTheme, string checkBoxTheme)
    {
        // Farklı temalar karıştırılabilir — tutarsız UI!
        var button   = new Button_Bad(buttonTheme);    // light
        var textBox  = new TextBox_Bad(textBoxTheme);  // dark  ← KARIŞIK!
        var checkBox = new CheckBox_Bad(checkBoxTheme); // light
    }
}

// Kullanım — hiçbir şey bunu engelleyemiyor!
badScreen.Render("light", "dark", "light"); // Tutarsız UI!
```

### Sonuçlar:

| Sorun | Açıklama |
|---|---|
| Tema karışıklığı | Light Button ile Dark TextBox yan yana gelebilir |
| Compiler korumuyor | Yanlış kombinasyon derleme anında fark edilemiyor |
| Yeni tema eklemek | Her bileşen için ayrı if-else bloğu gerekiyor |
| Bakım zorluğu | Hangi bileşenin hangi temaya ait olduğu takip edilemiyor |

---

## Doğru Kullanım — Abstract Factory Pattern

Her tema için tek bir factory — tüm bileşenler otomatik uyumlu:

```csharp
// Abstract Factory sözleşmesi
public interface IUIFactory
{
    string ThemeName { get; }
    IButton   CreateButton();
    ITextBox  CreateTextBox();
    ICheckBox CreateCheckBox();
}

// Light tema ailesi — hepsi birbiriyle uyumlu!
public class LightUIFactory : IUIFactory
{
    public string ThemeName => "Light";
    public IButton   CreateButton()   => new LightButton();
    public ITextBox  CreateTextBox()  => new LightTextBox();
    public ICheckBox CreateCheckBox() => new LightCheckBox();
}

// Dark tema ailesi — hepsi birbiriyle uyumlu!
public class DarkUIFactory : IUIFactory
{
    public string ThemeName => "Dark";
    public IButton   CreateButton()   => new DarkButton();
    public ITextBox  CreateTextBox()  => new DarkTextBox();
    public ICheckBox CreateCheckBox() => new DarkCheckBox();
}
```

Client tema detayından tamamen habersiz:

```csharp
public class UIApplication
{
    private readonly IButton   _button;
    private readonly ITextBox  _textBox;
    private readonly ICheckBox _checkBox;

    // Hangi tema gelirse gelsin — tüm bileşenler uyumlu!
    public UIApplication(IUIFactory factory)
    {
        _button   = factory.CreateButton();
        _textBox  = factory.CreateTextBox();
        _checkBox = factory.CreateCheckBox();
    }
}

// Tema değiştirmek tek satır!
var lightApp = new UIApplication(new LightUIFactory());
var darkApp  = new UIApplication(new DarkUIFactory());
```

---

## Factory Method vs Abstract Factory

| | Factory Method | Abstract Factory |
|---|---|---|
| Amaç | Tek bir ürün üretir | Birbiriyle uyumlu ürün ailesi üretir |
| Factory sayısı | Her ürün için bir factory | Her aile için bir factory |
| Ürün sayısı | Tek tip | Birden fazla tip (Button + TextBox + CheckBox) |
| Kullanım | Tek nesne oluşturmak | Uyumlu nesne grubu oluşturmak |
| Örnek | EmailNotificationFactory | LightUIFactory |

---

## Farkın Özeti

| | Abstract Factory İhlali | Abstract Factory Uyumlu |
|---|---|---|
| Tema tutarlılığı | Karıştırılabilir | Factory garantisi |
| Yeni tema eklemek | Her bileşende if-else | Yeni factory sınıfı ekle |
| Client bilgisi | Somut sınıfları biliyor | Sadece interface'i biliyor |
| OCP | İhlal | Korunuyor |
| Test edilebilirlik | Zor | Mock factory ile kolay |

---

## Testler

Testler **xUnit**, **Moq** ve **FluentAssertions** ile yazılmıştır.

### Kapsanan Senaryolar

**LightUIFactoryTests:**
- Tüm bileşenler doğru tipte üretiliyor
- Tüm bileşenler `IButton`, `ITextBox`, `ICheckBox` interface'lerine atanabilir
- Tüm bileşenlerin teması `Light` — tutarlılık garantisi
- Render, GetInput, Toggle metodları doğru çalışıyor
- Boş input → `ArgumentException`

**DarkUIFactoryTests:**
- Tüm bileşenler doğru tipte üretiliyor
- Tüm bileşenlerin teması `Dark` — tutarlılık garantisi
- Render metodları Dark tema içeriği döndürüyor

**UIApplicationTests:**
- `null` factory → `ArgumentNullException`
- Light/Dark factory ile `GetThemeName()` doğru değer döndürüyor
- `RenderUI()` ve `SimulateInteraction()` hata fırlatmıyor
- Mock factory ile herhangi bir `IUIFactory` implementasyonu çalışıyor
- Constructor'da tüm factory metodları bir kez çağrılıyor

---

## Design Patterns Serisi

| # | Pattern | Kategori | Durum |
|---|---|---|---|
| 1 | Singleton | Creational | ✅ Tamamlandı |
| 2 | Factory Method | Creational | ✅ Tamamlandı |
| 3 | Abstract Factory | Creational | ✅ Tamamlandı |
| 4 | Prototype | Creational | 🔜 Yakında |
| 5 | Builder | Creational | 🔜 Yakında |

---

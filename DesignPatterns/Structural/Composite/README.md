# Design Patterns #8 — Composite Pattern

> *"Nesneleri ağaç yapılarında birleştirerek parça-bütün hiyerarşilerini temsil eder. Composite, client'ların tekil nesneleri ve nesne kompozisyonlarını aynı şekilde ele almasını sağlar."*
> — Gang of Four (GoF)

---

## Composite Pattern Nedir?

Composite Pattern, tekil nesneleri (Leaf) ve nesne gruplarını (Composite) aynı interface üzerinden kullanmayı sağlar. Client bir nesnenin yaprak mı yoksa kompozit mi olduğunu bilmek zorunda kalmaz — her ikisiyle de aynı şekilde çalışır.

**Ne zaman kullanılır?**

- Parça-bütün hiyerarşisi modellenecekse
- Client tekil ve bileşik nesneleri aynı şekilde işleyecekse
- Ağaç yapısı recursive olarak işlenecekse
- Tip kontrolünden kaçınmak gerekiyorsa

**Gerçek hayat örnekleri:**
- Dosya sistemi — dosya ve klasörler
- Organizasyon şeması — çalışan ve departmanlar
- UI bileşen ağacı — widget ve konteynerlar
- Menü sistemi — menü öğesi ve alt menüler

---

## Kötü Kullanım — Composite İhlali

Dosya ve klasörler ayrı listede — her yerde tip kontrolü:

```csharp
public class FileSystemManager_Bad
{
    // Ayrı listeler — tip kontrolü şart
    private readonly List<File_Bad>      _files = new();
    private readonly List<Directory_Bad> _dirs  = new();

    public void Add(object item)
    {
        if (item is File_Bad file)           _files.Add(file);
        else if (item is Directory_Bad dir)  _dirs.Add(dir); //
    }

    public long CalculateTotalSize()
    {
        long total = 0;
        foreach (var file in _files) total += file.SizeInBytes;
        foreach (var dir in _dirs)   total += CalculateDirectorySize(dir); //
        return total;
    }

    // Her metotta ayrı recursive metot yazmak gerekiyor
    private long CalculateDirectorySize(Directory_Bad dir) { ... }
    private void PrintDirectory(Directory_Bad dir, string indent) { ... }
}
```

### Sonuçlar:

| Sorun | Açıklama |
|---|---|
| Tip kontrolü | Her metotta `if (item is File)` / `if (item is Directory)` |
| Kod tekrarı | Her işlem için ayrı recursive metot |
| OCP ihlali | Yeni tip = tüm metodlara dokun |
| Test zorluğu | Tip bağımlı kod izole test edilemiyor |

---

## Doğru Kullanım — Composite Pattern

Tek interface — tip kontrolü yok:

```csharp
// Ortak sözleşme — hem dosya hem klasör
public interface IFileSystemItem
{
    string Name { get; }
    long GetSize();
    void Print(string indent = "");
}

// Leaf — yaprak düğüm
public sealed class File : IFileSystemItem
{
    public long GetSize() => SizeInBytes; // Kendi boyutunu döner
}

// Composite — bileşik düğüm
public sealed class Directory : IFileSystemItem
{
    private readonly List<IFileSystemItem> _children = new();

    // Recursive — tip kontrolü yok!
    public long GetSize() => _children.Sum(child => child.GetSize());

    public void Print(string indent = "")
    {
        Console.WriteLine($"{indent} {Name}");
        foreach (var child in _children)
            child.Print(indent + "  "); // Dosya mı klasör mü? Önemli değil!
    }
}
```

Client her iki tiple aynı şekilde çalışıyor:

```csharp
public class FileSystemService
{
    // Tek metot — hem dosya hem klasör için!
    public long CalculateTotalSize(IFileSystemItem item)
        => item.GetSize();

    public void PrintStructure(IFileSystemItem item)
        => item.Print();
}

// Kullanım
service.CalculateTotalSize(file);      // Dosya
service.CalculateTotalSize(directory); // Klasör — aynı kod!
```

---

## Farkın Özeti

| | Composite İhlali | Composite Uyumlu |
|---|---|---|
| Tip kontrolü | Her metotta `if (is File) / if (is Directory)` | Hiç yok |
| Kod tekrarı | Her işlem için ayrı recursive metot | Tek metot |
| Yeni tip eklemek | Tüm metodlara dokun | Yeni sınıf ekle |
| Ağaç derinliği | Elle yönetilen recursive | Otomatik recursive |
| Test edilebilirlik | Tip bağımlı — zor | Interface mock'lanabilir |

---

## Testler

Testler **xUnit** ve **FluentAssertions** ile yazılmıştır.

### Kapsanan Senaryolar

**FileTests:**
- Constructor özellikleri doğru set ediyor
- Boş isim → `ArgumentException`, negatif boyut → `ArgumentOutOfRangeException`
- `GetSize()` kendi boyutunu döndürüyor
- Parent başlangıçta `null`, `SetParent()` güncelliyor
- `IFileSystemItem` interface'ini implemente ediyor

**DirectoryTests:**
- `Add()` → ChildCount artıyor, parent set ediliyor
- `Remove()` → ChildCount azalıyor, parent temizleniyor
- `GetSize()` boş klasörde sıfır döner
- `GetSize()` dosyaların toplamını döner
- `GetSize()` **nested klasörlerde recursive çalışıyor**
- `GetSize()` derin iç içe yapıda doğru hesaplıyor
- Hem dosya hem klasör aynı interface ile eklenebiliyor

**FileSystemServiceTests:**
- `CalculateTotalSize()` dosya ve klasör için doğru çalışıyor
- `FindByName()` dosya ve klasör buluyor, büyük/küçük harf duyarsız
- `FindAll()` predicate ile eşleşen öğeleri buluyor
- Dosya ve klasör **aynı interface** ile işleniyor — Composite garantisi

---

## Kullanılan Teknolojiler

| Teknoloji | Versiyon | Amaç |
|---|---|---|
| .NET | 8.0 | Ana platform |
| xUnit | 2.5.3 | Test framework |
| FluentAssertions | 8.9.0 | Okunabilir assertion'lar |

---

## 📚 Design Patterns Serisi

| # | Pattern | Kategori | Durum |
|---|---|---|---|
| 1 | Flyweight | Structural | ✅ Tamamlandı |
| 2 | Adapter | Structural | ✅ Tamamlandı |
| 3 | Composite | Structural | ✅ Tamamlandı |
| 4 | Facade | Structural | 🔜 Yakında |
| 5 | Proxy | Structural | 🔜 Yakında |
| 6 | Decorator | Structural | 🔜 Yakında |
| 7 | Bridge | Structural | 🔜 Yakında |

---

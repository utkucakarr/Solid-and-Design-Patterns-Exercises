# Memento Pattern — Doküman Editörü Undo/Redo Sistemi

> *"Without violating encapsulation, capture and externalize an object's internal state so that the object can be restored to this state later."*
> *"Kapsüllemeyi (encapsulation) bozmadan bir nesnenin iç durumunu yakalayıp dışa aktarın; böylece nesne daha sonra bu duruma geri döndürülebilir."*
> — **Gang of Four, Design Patterns**

---

## Pattern Nedir?

**Memento Pattern**, bir nesnenin iç state'ini kapsüllemeyi bozmadan dışarı çıkarıp saklayan ve gerektiğinde geri yükleyen davranışsal bir tasarım kalıbıdır. Üç temel aktörü vardır: state'i tutan **Originator**, snapshot'ı saklayan **Memento** ve yığınları yöneten **Caretaker**.

### Ne Zaman Kullanılır?

- Undo/Redo mekanizması gereken her yerde
- Nesnenin önceki state'ine geri dönülmesi gerektiğinde
- State'in dışarıya açılmadan saklanması gerektiğinde (encapsulation korunmalı)
- İşlem geçmişinin tutulması gereken sistemlerde
- Snapshot/checkpoint mekanizması kurmak istendiğinde

### Gerçek Hayat Örnekleri

| Domain | Originator | Memento | Caretaker |
|---|---|---|---|
| Metin Editörü | Document | DocumentMemento | DocumentHistory |
| Oyun | GameState | SavePoint | SaveManager |
| Form Wizard | FormData | FormSnapshot | WizardHistory |
| Veritabanı | Transaction | TransactionLog | TransactionManager |
| Grafik Editör | Canvas | CanvasSnapshot | UndoManager |

---

## Kötü Kullanım (Violation)

```csharp
public class Document_Bad
{
    // Geçmiş yığını doğrudan Document içinde — SRP ihlali
    private readonly Stack<DocumentSnapshot> _undoStack = new();
    private readonly Stack<DocumentSnapshot> _redoStack = new();

    public void SetTitle(string title)
    {
        // Redo stack temizlenmiyor — tutarsız state
        _undoStack.Push(new DocumentSnapshot(Title, Content, Tags));
        Title = title;
    }

    public void AddTag(string tag)
    {
        // Tags referans kopyası — snapshot sonrası
        // yapılan değişiklik geçmişi de etkiliyor
        _undoStack.Push(new DocumentSnapshot(Title, Content, Tags));
        Tags.Add(tag);
    }
}

// Snapshot tamamen dışarıya açık — encapsulation yok
public class DocumentSnapshot
{
    public string Title { get; set; }      // 🚨 set erişilebilir
    public string Content { get; set; }    // 🚨 set erişilebilir
    public List<string> Tags { get; set; } // 🚨 referans kopyası
    public DateTime SavedAt { get; set; }  // 🚨 dışarıdan değiştirilebilir
}
```

### Violation Sonuçları

| Sorun | Açıklama |
|---|---|
| SRP İhlali | `Document_Bad` hem içeriği hem de geçmişi yönetiyor |
| Encapsulation İhlali | `DocumentSnapshot` tamamen dışarıya açık |
| Deep Copy Yok | Tags referans kopyası — snapshot sonrası değişiklikler geçmişi bozuyor |
| Redo Tutarsızlığı | `SetTitle/SetContent` sonrası Redo stack temizlenmiyor |
| Test Edilemezlik | Caretaker Document'a gömülü — ayrı test edilemiyor |

---

## Doğru Kullanım (Implementation)

```csharp
// Originator — kendi snapshot'ını oluşturur ve geri yükler
public class Document : IDocument
{
    public IMemento Save(string label)
    {
        // Deep copy ile snapshot — dış değişikliklerden korunur
        var state = new DocumentState(_title, _content, _tags);
        return new DocumentMemento(state, label);
    }

    public void Restore(IMemento memento)
    {
        if (memento is not DocumentMemento documentMemento)
            throw new InvalidOperationException("Geçersiz memento tipi");

        // internal State — yalnızca Originator erişebilir
        _title = documentMemento.State.Title;
        _content = documentMemento.State.Content;
        _tags.Clear();
        _tags.AddRange(documentMemento.State.Tags); // ✅ Deep copy
    }
}

// Memento — State dışarıya kapalı
public sealed class DocumentMemento : IMemento
{
    internal DocumentState State { get; } // internal — assembly dışı erişim yok
    public string Label { get; }
    public DateTime SavedAt { get; }
}

// Caretaker — undo/redo yığınlarını yönetir, Document'ı bilmez
public class DocumentHistory : IDocumentHistory
{
    public void Push(IMemento memento)
    {
        _undoStack.Push(memento);
        _redoStack.Clear(); // Yeni değişiklik → Redo geçersiz
    }
}

// Editor — aksiyonları orkestre eder
public class DocumentEditor : IDocumentEditor
{
    public DocumentResult SetTitle(string title)
    {
        // Önce snapshot al, sonra değiştir
        _history.Push(_document.Save($"Başlık: '{_document.Title}' → '{title}'"));
        _document.SetTitle(title);
        return BuildSuccess($"Başlık güncellendi.");
    }
}
```

---

## Pattern'in Yaptığı İşlem

| Adım | Aktör | Eylem |
|---|---|---|
| 1 | `DocumentEditor` | Değişiklik öncesi `_document.Save(label)` çağırır |
| 2 | `Document` (Originator) | `DocumentState` ile deep copy snapshot oluşturur |
| 3 | `DocumentMemento` | State'i `internal` olarak saklar — dışarıya kapalı |
| 4 | `DocumentHistory` (Caretaker) | Memento'yu undo stack'e ekler, redo'yu temizler |
| 5 | `DocumentEditor` | Asıl değişikliği uygular |
| 6 | Undo çağrısında | Editor mevcut state'i redo'ya taşır, son memento'yu geri yükler |
| 7 | Redo çağrısında | Editor mevcut state'i undo'ya taşır, son redo memento'yu geri yükler |

---

## Farkın Özeti

| Özellik | Bad | Good |
|---|---|---|
| Sorumluluk | Document hem içeriği hem geçmişi yönetiyor | Document içerik, History geçmiş yönetiyor |
| Encapsulation | Snapshot tamamen dışarıya açık | `internal State` — yalnızca Originator erişebilir |
| Deep Copy | Tags referans kopyası | `DocumentState` ile tam izolasyon |
| Redo Tutarlılığı | Yeni değişiklikte Redo temizlenmiyor | `Push` her zaman Redo'yu temizliyor |
| Test Edilebilirlik | Caretaker Document'a gömülü | Her sınıf bağımsız test edilebilir |
| SRP | İhlal | Her sınıf tek sorumluluğa sahip |

---

## Testler

### Neden Moq Gerekmedi?

Memento Pattern'de tüm aktörler (Originator, Memento, Caretaker) saf in-memory nesnelerdir ve dış servise bağımlılık yoktur. Bu nedenle Moq yerine doğrudan somut sınıflar test edildi. Tek mock ihtiyacı, `Document.Restore` metodunun geçersiz memento tipiyle nasıl davrandığını test etmek için yazılan `InvalidMemento` yardımcı sınıfıdır.

### Kapsanan Senaryolar

| Kategori | Test Sayısı | Örnekler |
|---|---|---|
| Happy Path | 5 | SetTitle, SetContent, AddTag, RemoveTag, GetCurrentState |
| Undo Senaryoları | 6 | Başlık geri alma, içerik geri alma, çoklu adım, boş stack |
| Redo Senaryoları | 5 | Undo sonrası redo, çoklu adım, yeni değişiklik sonrası |
| Stack Doğrulama | 4 | UndoCount, RedoCount, Push sonrası temizlik |
| Deep Copy İzolasyonu | 3 | Snapshot izolasyonu, tutarlı undo/redo zinciri |
| Document (Originator) | 3 | Save label, Restore state, geçersiz memento |
| DocumentHistory (Caretaker) | 4 | Push, PopUndo/Redo null, CanUndo/CanRedo |
| Guard Clause | 5 | Boş başlık, null içerik, boş tag |
| Constructor Null Guard | 7 | Editor, Document, History null kontrolleri |

---

## SOLID ile Bağlantısı

| Prensip | Bağlantı |
|---|---|
| **S**RP | `Document` içeriği, `DocumentHistory` geçmişi, `DocumentEditor` aksiyonları yönetiyor |
| **O**CP | Yeni aksiyonlar `DocumentEditor`'a eklenir, Originator/Caretaker değişmez |
| **L**SP | `IMemento` implementasyonları birbirinin yerine geçebilir |
| **I**SP | `IOriginator`, `IDocument`, `IDocumentHistory` ayrı arayüzlerde |
| **D**IP | `DocumentEditor` somut sınıflara değil `IDocument` ve `IDocumentHistory` arayüzlerine bağımlı |

---

## Diğer Pattern'lerle İlişkisi

| Pattern | İlişki |
|---|---|
| **Command** | Command Pattern de undo/redo destekler; fark şudur: Command her işlemi bir nesneye sararken, Memento tüm state'in snapshot'ını alır. Command daha granüler, Memento daha bütünsel bir yaklaşımdır. |
| **State** | State Pattern nesnenin davranışını değiştirirken, Memento nesnenin state'ini saklar ve geri yükler. İkisi birlikte kullanılabilir: State geçişleri Memento ile geri alınabilir hale getirilebilir. |
| **Iterator** | Caretaker snapshot listesini iterate ederek geçmiş adımların üzerinde gezinmek için Iterator Pattern'den yararlanabilir. |
| **Prototype** | Memento'nun deep copy ihtiyacı Prototype Pattern ile karşılanabilir. Originator `Clone()` ile kendi kopyasını üretip saklayabilir. |
| **Observer (9. Pattern)** | Memento ile Observer birleştirildiğinde, state değişimleri snapshot alınırken observer'lar da tetiklenebilir. Örneğin her Undo/Redo işleminde UI'ın güncellenmesi için Observer kullanılabilir. |

---

## Kullanılan Teknolojiler

| Teknoloji | Versiyon | Kullanım Amacı |
|---|---|---|
| C# / .NET | 8.0 | Ana geliştirme platformu |
| C# `record` | 8.0 | Immutable `DocumentState` için |
| C# `internal` | 8.0 | `DocumentMemento.State` encapsulation için |
| Microsoft.Extensions.DependencyInjection | 10.0.6 | DI container |
| xUnit | 2.5.3 | Test framework |
| FluentAssertions | 8.9.0 | Okunabilir assertion'lar |

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
| 7 | Mediator | Behavioral | 🔜 Yakında |
| 8 | Chain Of Responsibility | Behavioral | 🔜 Yakında |
| 9 | Visitor | Behavioral | 🔜 Yakında |
| 10 | State | Behavioral | 🔜 Yakında |
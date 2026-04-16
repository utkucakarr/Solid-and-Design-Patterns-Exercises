# SOLID #3 — Liskov Substitution Principle (LSP)

> *"Alt sınıflar, üst sınıflarının yerine geçtiğinde program doğru çalışmaya devam etmelidir."*
> — Barbara Liskov, 1987

---

## Prensip Nedir?

Liskov Substitution Principle, bir alt sınıfın üst sınıfının **tüm sözleşmelerini eksiksiz yerine getirmesi** gerektiğini söyler.

Eğer bir alt sınıf, üst sınıfın bir metodunu `NotSupportedException` fırlatarak geçersiz kılıyorsa — bu LSP ihlalidir. Çünkü o sınıf, üst sınıfın yerine güvenle **geçemiyor** demektir.

**Test sorusu:** Üst sınıf referansını alt sınıfla değiştirdiğinde program hâlâ doğru çalışıyor mu?

- Evet → LSP uyumlu
- Hayır / Exception fırlatıyor → LSP ihlali

## Kötü Kullanım — LSP İhlali

`Employee_Bad` abstract sınıfı, tüm çalışan tiplerine aynı sözleşmeyi dayatıyor:

```csharp
public abstract class Employee_Bad
{
    public abstract decimal CalculateSalary();
    public abstract decimal CalculateBonus();    // Her çalışan prim alabilir mi? HAYIR!
    public abstract decimal CalculateOvertime(); // Her çalışan fazla mesai alabilir mi? HAYIR!
}
```

Stajyer ve sözleşmeli çalışanlar bu sözleşmeyi yerine getiremiyor:

```csharp
public class Intern_Bad : Employee_Bad
{
    public override decimal CalculateBonus()
        => throw new NotSupportedException("Stajyerler prim alamaz!");

    public override decimal CalculateOvertime()
        => throw new NotSupportedException("Stajyerler fazla mesai alamaz!");
}
```

### Sonuç: Runtime'da patlıyor!

```csharp
var employees = new List<Employee_Bad>
{
    new FullTimeEmployee_Bad("Ahmet", 30000),
    new Contractor_Bad("Ayşe", 20000),
    new Intern_Bad("Mehmet", 10000)  
};

foreach (var emp in employees)
    emp.CalculateBonus();
```

`Intern_Bad`, `Employee_Bad`'in yerine geçtiğinde program çöküyor. **Bu direkt LSP ihlalidir.**

---

## Doğru Kullanım — LSP Uyumlu Yapı

Her yetenek ayrı bir interface ile temsil ediliyor. Çalışanlar **yalnızca hak ettikleri sözleşmeleri** implemente ediyor:

```csharp
public interface IEmployee        { decimal CalculateSalary(); }
public interface IBonusEligible   { decimal CalculateBonus(); }
public interface IOvertimeEligible { decimal CalculateOvertime(); }
```

```csharp
// Tüm çalışanlar — sadece maaş, güvenli!
var allEmployees = new List<IEmployee> { fullTime, contractor, intern };

// Sadece prim alabilenler — Intern giremez, compiler engeller!
var bonusEligibles = new List<IBonusEligible> { fullTime, contractor };

// Sadece fazla mesai alabilenler
var overtimeEligibles = new List<IOvertimeEligible> { fullTime };
```

Artık `Intern` yanlış listeye **giremez bile.** Hata runtime'da değil, **derleme anında** yakalanır.

---

## Farkın Özeti

| | LSP İhlali | LSP Uyumlu |
|---|---|---|
| Hata zamanı | Runtime — program çalışırken patlıyor | Compile time — derleme anında yakalanıyor |
| Yanlış kullanım | Liste içine girebilir, fark edilmez | Compiler engeller, listeye giremez |
| Sözleşme | Herkese aynı — yerine getirilemiyor | Her sınıf sadece söz verdiğini yapıyor |
| Güvenlik | try-catch ile kapatmak gerekiyor | Tip sistemi koruyor |

---

## Testler

Testler **xUnit** ve **FluentAssertions** ile yazılmıştır.

### Kapsanan Senaryolar

**FullTimeEmployeeTests:**
- Maaş, prim ve fazla mesai doğru hesaplanıyor.
- `IEmployee`, `IBonusEligible`, `IOvertimeEligible` atanabilir.
- Guard clause — boş isim ve negatif maaş engelleniyor.

**InternTests:**
- Maaş doğru hesaplanıyor.
- `IEmployee` atanabilir.
- `IBonusEligible` ve `IOvertimeEligible` **atanamıyor** — LSP garantisi.
- `IEmployee` referansıyla kullanıldığında exception yok.

**ContractorTests:**
- Maaş ve prim doğru hesaplanıyor.
- `IEmployee` ve `IBonusEligible` atanabilir.
- `IOvertimeEligible` **atanamıyor** — LSP garantisi.

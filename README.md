# Solid-and-Design-Patterns-Exercises
Solid Prensipleri ve Design Patterns Öğrenme Aşamasındaki Çalışmalar

SOLID & Design Patterns Exercises
Bu depo (repository), yazılım geliştirmede sürdürülebilir, test edilebilir ve temiz kod (clean code) yazımı için kritik öneme sahip olan SOLID prensipleri ve Tasarım Kalıpları (Design Patterns) üzerine yaptığım çalışmaları içermektedir.

Her bir prensip, gerçek dünya senaryolarına dayanan küçük projeler ve kapsamlı Unit Test çalışmalarıyla desteklenmiştir.

Yol Haritası (Roadmap)
[x] SRP - Single Responsibility Principle (Tamamlandı)

[ ] OCP - Open-Closed Principle (Tamamlandı )

[ ] LSP - Liskov Substitution Principle (Sıradaki)

[ ] ISP - Interface Segregation Principle

[ ] DIP - Dependency Inversion Principle

[ ] Design Patterns (Planlanıyor)

1. Single Responsibility Principle (SRP)
Tanım: Bir sınıfın (veya metodun) değişmek için tek bir nedeni olmalıdır. Yani bir sınıf sadece tek bir işten sorumlu olmalıdır.

Uygulama: Emlak Portalı (Real Estate)
Bu çalışmada, bir emlak ilanının sisteme eklenme süreci SRP'ye uygun olarak parçalara ayrılmıştır:

Property (Model): Sadece ilan verilerini taşır.

PropertyValidator (Validation): İlanın iş kurallarına (fiyat > 0, başlık uzunluğu vb.) uygunluğunu denetler.

PropertyRepository (Data Access): Verinin kalıcı hale getirilmesinden (dosya sistemine kayıt) sorumludur.

ConsoleLogger (Logging): Sistemsel bilgilendirmeleri kullanıcıya sunar.

PropertyManager (Orchestration): Yukarıdaki tüm birimleri bir araya getirerek iş akışını yönetir.

Unit Test Süreci
Kodun doğruluğunu kanıtlamak için xUnit ve Moq kütüphaneleri kullanılmıştır:

Mantık Testleri: PropertyValidator için farklı veri senaryoları ([Theory]) test edildi.

Davranış Testleri: PropertyManager test edilirken Moq ile bağımlılıklar taklit edildi. Validasyon hatalıysa kaydın asla yapılmadığı (Times.Never) doğrulandı.

Gemini şunu dedi:
Harika bir fikir! SRP için yaptığın o düzenli ve açıklayıcı yapıyı OCP için de devam ettirmen, reponun bütünlüğünü korur ve inceleyen kişiye "Bu arkadaş sadece kod yazmıyor, yazdığı mimariyi dokümante etmeyi de biliyor" dedirtir.

İşte SRP formatına uygun olarak hazırladığım Open/Closed Principle (OCP) bölümü. Bunu README dosyanda SRP'nin hemen altına ekleyebilirsin:

2. Open/Closed Principle (OCP)
Tanım: Yazılım varlıkları (sınıflar, modüller, fonksiyonlar vb.) genişletilmeye açık (Open), ancak değişime kapalı (Closed) olmalıdır. Yani yeni bir özellik eklemek istediğimizde mevcut, çalışan koda dokunmak yerine sisteme yeni parçalar ekleyebilmeliyiz.

Uygulama: İndirim Hesaplama Sistemi (Discount System)
Bu çalışmada, bir e-ticaret senaryosundaki indirim hesaplama mantığı, her yeni kampanya döneminde mevcut kodu bozma riskini (regression) ortadan kaldırmak için OCP'ye uygun hale getirilmiştir:

IDiscountStrategy (Abstraction): Tüm indirim türlerinin uyması gereken şablonu belirleyen arayüzdür. Sistemin "ne yapacağını" tanımlar.

Standard/Premium/VipDiscount (Strategies): Somut indirim kurallarıdır. Her indirim türü kendi sınıfında izole edilmiştir. Yeni bir indirim türü (örneğin: BlackFridayDiscount) eklemek için mevcut sınıflara dokunulmaz; sadece bu arayüzden türeyen yeni bir sınıf eklenir.

DiscountCalculator (Service): Hesaplama motoru (Orchestrator). Hangi stratejinin seçileceğiyle veya hesaplama detaylarıyla ilgilenmez; sadece kendisine verilen stratejiyi çalıştırır. Bu sayede bu sınıf "Değişime Kapalı" hale getirilmiştir.

Unit Test Süreci
Kodun esnekliğini ve doğruluğunu kanıtlamak için xUnit kütüphanesi kullanılmıştır:

Strateji Testleri: Her bir indirim sınıfının (Standard, Premium, VIP) kendi matematiksel mantığı bağımsız olarak test edilmiştir.

Motor Testi: DiscountCalculator sınıfının, kendisine verilen herhangi bir stratejiyi doğru şekilde tetikleyip sonuç döndürdüğü [Theory] ve [InlineData] senaryoları ile doğrulanmıştır.

# Solid-and-Design-Patterns-Exercises
Solid Prensipleri ve Design Patterns Öğrenme Aşamasındaki Çalışmalar

SOLID & Design Patterns Exercises
Bu depo (repository), yazılım geliştirmede sürdürülebilir, test edilebilir ve temiz kod (clean code) yazımı için kritik öneme sahip olan SOLID prensipleri ve Tasarım Kalıpları (Design Patterns) üzerine yaptığım çalışmaları içermektedir.

Her bir prensip, gerçek dünya senaryolarına dayanan küçük projeler ve kapsamlı Unit Test çalışmalarıyla desteklenmiştir.

Yol Haritası (Roadmap)
[x] SRP - Single Responsibility Principle (Tamamlandı)

[ ] OCP - Open-Closed Principle (Sıradaki )

[ ] LSP - Liskov Substitution Principle

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

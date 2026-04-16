# SOLID ve Design Patterns Çalışmaları

Bu proje içerisinde, sürdürülebilir ve esnek yazılım mimarisinin temel taşlarından olan **SOLID Prensipleri** ile yazılım geliştirme süreçlerinde sıkça karşılaşılan problemlere standart çözümler getiren **Tasarım Kalıpları (Design Patterns)** üzerine pratik çalışmalar yapılmıştır.

Amacım; temiz (clean) kod yazma pratiklerini, bağımlılıkların yönetimini ve nesne yönelimli programlama (OOP) kavramlarını senaryolar üzerinden uygulamalı olarak göstermektir.

---

## Önemli Bilgilendirme

Proje içerisindeki her bir tasarım kalıbı ve SOLID prensibi kendi bağımsız klasöründe izole edilmiştir. 

**Her çalışmanın klasörü içerisinde o konuya özel detaylı bir `README.md` dosyası bulunmaktadır.** Kodları incelemeden önce, ilgili klasörün içerisindeki README dosyasını okumanız konunun teorik altyapısını, senaryosunu ve implementasyon detaylarını anlamanıza büyük ölçüde yardımcı olacaktır.

---

## Proje Klasör Yapısı

Projenin genel dosya hiyerarşisi ve içerdiği konular aşağıdaki gibidir:

```text
📦 Solid-and-Design-Patterns-Exercises
 ┣ 📂 DesignPatterns
 ┃ ┣ 📂 Creational
 ┃ ┃ ┣ 📂 AbstractFactory
 ┃ ┃ ┣ 📂 Builder
 ┃ ┃ ┣ 📂 FactoryMethod
 ┃ ┃ ┣ 📂 Prototype
 ┃ ┃ ┗ 📂 Singleton
 ┃ ┣ 📂 Structural
 ┃ ┃ ┣ 📂 Adapter
 ┃ ┃ ┣ 📂 Bridge
 ┃ ┃ ┣ 📂 Composite
 ┃ ┃ ┣ 📂 Decorator
 ┃ ┃ ┣ 📂 Facade
 ┃ ┃ ┣ 📂 Flyweight
 ┃ ┃ ┗ 📂 Proxy
 ┃ ┗ 📂 Behavioral (🔜 Yakında eklenecek)
 ┗ 📂 SOLID
   ┣ 📂 DIP(Dependency-Inversion-Principle)
   ┣ 📂 ISP(Interface-Segregation-Principle)
   ┣ 📂 LSP(Liskov-Substitution-Principle)
   ┣ 📂 OCP(Open-Closed-Principle)
   ┗ 📂 SRP(Single-Responsibility-Principle)

🛠️ Projeyi İndirme ve Çalıştırma
Projeyi bilgisayarınıza indirip kodları yerel ortamınızda incelemek isterseniz aşağıdaki adımları izleyebilirsiniz:

1. Projeyi Klonlayın:
Terminalinizi açın ve aşağıdaki komutu çalıştırarak projeyi bilgisayarınıza indirin:

Bash
git clone [[https://github.com/utkucakarr/Solid-and-Design-Patterns-Exercises.git](https://github.com/utkucakarr/Solid-and-Design-Patterns-Exercises.git)](https://github.com/utkucakarr/Solid-and-Design-Patterns-Exercises.git)

2. Proje Dizinine Geçiş Yapın:
Bash
cd Solid-and-Design-Patterns-Exercises

Çalışmaları incelediğiniz için teşekkürler. Her türlü geri bildirim ve katkıya açığım. İyi kodlamalar!

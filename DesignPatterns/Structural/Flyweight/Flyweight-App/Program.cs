using Flyweight_Implementation.Forests;
using Flyweight_Violation.Forest;

Console.WriteLine("╔══════════════════════════════════════╗");
Console.WriteLine("║   FLYWEIGHT İHLALİ — CANLI DEMO       ║");
Console.WriteLine("╚══════════════════════════════════════╝\n");

var badForest = new ForestBad();

Console.WriteLine("--- 9 Ağaç Eklenliyor (Her biri ayrı texture ve color verisi) ---\n");

badForest.PlantTree("Oak", "oak_texture", "Koyu Yeşiş", 10, 20, 5);
badForest.PlantTree("Oak", "oak_texture", "Koyu Yeşil", 30, 40, 7);
badForest.PlantTree("Oak", "oak_texture", "Koyu Yeşil", 50, 60, 6);
badForest.PlantTree("Pine", "pine_texture", "Koyu Mavi", 15, 25, 8);
badForest.PlantTree("Pine", "pine_texture", "Koyu Mavi", 35, 45, 9);
badForest.PlantTree("Pine", "pine_texture", "Koyu Mavi", 55, 65, 7);
badForest.PlantTree("Birch", "birch_texture", "Açık Sarı", 20, 30, 4);
badForest.PlantTree("Birch", "birch_texture", "Açık Sarı", 40, 50, 5);
badForest.PlantTree("Birch", "birch_texture", "Açık Sarı", 60, 70, 6);

Console.WriteLine();
Console.WriteLine($" Toplam ağaç: {badForest.TreeCount}");
Console.WriteLine("  Sorunlar:");
Console.WriteLine("  -> 9 ağaç = 9 ayrı texture ve color nesnesi!");
Console.WriteLine("  -> 1.000.000 ağaç olsaydı = 1.000.000 texture kopyası");
Console.WriteLine("  -> Bellek kullanımı O(n) — ağaç sayısıyla doğru orantılı\n");

Console.WriteLine("╔══════════════════════════════════════╗");
Console.WriteLine("║   FLYWEIGHT ÇÖZÜMÜ — DOĞRU YAKLAŞIM   ║");
Console.WriteLine("╚══════════════════════════════════════╝\n");

var forest = new Forest();

Console.WriteLine("--- 9 Ağaç Ekleniyor (TreeType'lar paylaşılıyor) ---\n");

forest.PlantTree("Oak", "Koyu Yeşil", "oak_texture", 10, 20, 5);
forest.PlantTree("Oak", "Koyu Yeşil", "oak_texture", 30, 40, 7);
forest.PlantTree("Oak", "Koyu Yeşil", "oak_texture", 50, 60, 6);
forest.PlantTree("Pine", "Koyu Mavi", "pine_texture", 15, 25, 8);
forest.PlantTree("Pine", "Koyu Mavi", "pine_texture", 35, 45, 9);
forest.PlantTree("Pine", "Koyu Mavi", "pine_texture", 55, 65, 7);
forest.PlantTree("Birch", "Açık Sarı", "birch_texture", 20, 30, 4);
forest.PlantTree("Birch", "Açık Sarı", "birch_texture", 40, 50, 5);
forest.PlantTree("Birch", "Açık Sarı", "birch_texture", 60, 70, 6);

forest.Render();
forest.PrintMemoryStats();

Console.WriteLine();
Console.WriteLine("9 ağaç -> sadece 3 TreeType nesnesi!");
Console.WriteLine("1.000.000 ağaç olsaydı -> yine sadece 3 TreeType nesnesi!");
Console.WriteLine("Bellek kullanımı O(k) — benzersiz tip sayısıyla orantılı\n");

Console.WriteLine("--- SONUÇ ---");
Console.WriteLine(" Bad  -> Her ağaç kendi verisini tutuyor — O(n) bellek");
Console.WriteLine(" Good -> Paylaşılan TreeType — O(k) bellek (k << n)\n");
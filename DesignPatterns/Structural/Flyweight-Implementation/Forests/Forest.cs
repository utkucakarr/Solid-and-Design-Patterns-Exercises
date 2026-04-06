using Flyweight_Implementation.Factory;
using Flyweight_Implementation.Models;

namespace Flyweight_Implementation.Forests
{
    // Forest — ağaçları yönetiyor, TreeFactory üzerinden TreeType paylaşıyor
    public class Forest
    {
        private readonly List<Tree> _trees = new();
        private readonly TreeFactory _treeFactory = new();

        public int TreeCount => _trees.Count();
        public int UniqueTreeTypes => _treeFactory.UniqueTreeTypeCount;


        public void PlantTree(
            string typeName, 
            string color, 
            string texture, 
            int x, 
            int y, 
            int size)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(x, nameof(x));
            ArgumentOutOfRangeException.ThrowIfNegative(y, nameof(y));
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(size, nameof(size));

            // Factory aynı tip için mevcut TreeType'ı döndürüyor
            var treeType = _treeFactory.GetTreeType(typeName, color, texture);
            var tree = new Tree(x, y, size, treeType);

            _trees.Add(tree);
        }

        public void Render()
        {
            Console.WriteLine($"\n─── Orman Render Ediliyor " +
                              $"({TreeCount} ağaç, {UniqueTreeTypes} benzersiz tip) ---\n");

            foreach (var tree in _trees)
                tree.Render();
        }

        public void PrintMemoryStats()
        {
            Console.WriteLine($"\n--- Bellek İstatistikleri ---");
            Console.WriteLine($"  Toplam ağaç sayısı       : {TreeCount}");
            Console.WriteLine($"  Benzersiz TreeType sayısı: {UniqueTreeTypes}");
            Console.WriteLine($"  Paylaşılan nesne oranı   : " +
                              $"%{(1 - (double)UniqueTreeTypes / TreeCount) * 100:F1}");
        }
    }
}

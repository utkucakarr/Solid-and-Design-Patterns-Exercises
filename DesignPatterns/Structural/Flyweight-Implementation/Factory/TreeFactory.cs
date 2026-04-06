using Flyweight_Implementation.Models;

namespace Flyweight_Implementation.Factory
{
    // Flyweight Factory — TreeType nesnelerini cache'leyip paylaştırıyor
    public class TreeFactory
    {
        private readonly Dictionary<string, TreeType> _treeTypes = new();

        public TreeType GetTreeType(string name, string color, string texture)
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(name, nameof(name));
            ArgumentNullException.ThrowIfNullOrWhiteSpace(color, nameof(color));
            ArgumentNullException.ThrowIfNullOrWhiteSpace(texture, nameof(texture));

            var key = $"{name}_{color}_{texture}";

            if (!_treeTypes.TryGetValue(key, out var treeType))
            {
                treeType = new TreeType(name, color, texture);
                _treeTypes[key] = treeType;
                Console.WriteLine($"[Factory] Yeni TreeType oluşturuldu: {treeType}");
            }
            else
            {
                Console.WriteLine($"[Factory] Mevcut TreeType kullanıldı: {treeType}");
            }

            return treeType;
        }

        // Cache istatistikleri
        public int UniqueTreeTypeCount => _treeTypes.Count;

        public IReadOnlyCollection<string> GetCachedTypeNames()
            => _treeTypes.Keys.ToList().AsReadOnly();
    }
}

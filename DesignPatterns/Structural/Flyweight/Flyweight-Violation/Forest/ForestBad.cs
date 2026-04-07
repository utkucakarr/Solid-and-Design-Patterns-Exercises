namespace Flyweight_Violation.Forest
{
    // Her ağaç sıfırdan oluşturuluyor — bellek israfı
    public class ForestBad
    {
        private readonly List<TreeBad> _trees = new();

        public void PlantTree(string type, string texture, string color, int x, int y, int size)
        {
            // Her çağrıda yeni texture ve color verisi oluşturuluyor
            var tree = new TreeBad(type, texture, color, x, y, size);
            _trees.Add(tree);
        }

        public int TreeCount => _trees.Count();

        public void Render()
        {
            foreach (var tree in _trees)
            {
                tree.Render();
            }
        }
    }
}

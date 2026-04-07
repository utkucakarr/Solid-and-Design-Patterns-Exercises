namespace Flyweight_Violation.Forest
{
    // Flyweight ihliali - her apaç tüm verileri kendi içinde tutuyor.
    // 1.000.000 ağaç = 1.000.000 ayrı texture, color, type verisi!
    public class TreeBad
    {
        // Intrinsic (paylaşılabilir) veriler — her nesnede tekrar ediliyor
        public string TreeType { get; set; }

        public string Texture { get; }

        public string Color { get; }

        /// Extrinsic (nesneye) özel veriler
        public int X { get; }
        public int Y { get; }
        public int Size { get; }

        // Her ağaç kendi texture ve color verisini yükleyip saklıyor
        public TreeBad(string treeType, string texture, string color, int x, int y, int size)
        {
            TreeType = treeType;
            Texture = texture;  // 1M ağaç = 1M texture kopyası!
            Color = color;
            X = x;
            Y = y;
            Size = size;
        }

        public void Render()
            => Console.WriteLine($"[{TreeType}] Konum: ({X},{Y}) | " +
                             $"Boyut: {Size} | Renk: {Color}");
    }
}

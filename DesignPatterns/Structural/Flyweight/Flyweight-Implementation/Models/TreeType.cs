namespace Flyweight_Implementation.Models
{
    // Flyweight — paylaşılan intrinsic veriler burada
    // Tüm aynı tipteki ağaçlar bu tek nesneyi paylaşır!
    public sealed class TreeType
    {
        // Intrinsic state — değişmeyen, paylaşılabilir veriler
        public string Name { get; }
        public string Color { get; }
        public string Texture { get; set; }

        internal TreeType(string name, string color, string texture)
        {
            Name = name;
            Color = color;
            Texture = texture;
        }

        // Extrinsic veriler (x, y, size) dışarıdan geliyor
        // TreeType kendi içinde tutmuyor — sadece render için kullanıyor
        public void Render(int x, int y, int size)
        {
            Console.WriteLine($"[{Name}] Konum: ({x},{y}) | " +
                  $"Boyut: {size} | Renk: {Color} | " +
                  $"Doku: {Texture}");
        }

        public override string ToString()
            => $"TreeTyp[{Name}, {Color}, {Texture}]";
    }
}
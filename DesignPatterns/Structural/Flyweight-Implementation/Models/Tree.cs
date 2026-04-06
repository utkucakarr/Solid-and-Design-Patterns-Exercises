namespace Flyweight_Implementation.Models
{
    // Tree — sadece extrinsic (nesneye özel) verileri tutuyor
    // Intrinsic veriler TreeType'ta — paylaşılıyor!
    public class Tree
    {
        // Extrinsic state — her ağaca özel
        public int X { get; }
        public int Y { get; }
        public int Size { get; }

        // Flyweight referansı — paylaşılan nesneye işaret eder
        private readonly TreeType _treeType;

        public Tree(int x, int y, int size, TreeType treeType)
        {
            ArgumentNullException.ThrowIfNull(treeType, nameof(treeType));
            ArgumentOutOfRangeException.ThrowIfNegative(x, nameof(x));
            ArgumentOutOfRangeException.ThrowIfNegative(y, nameof(y));
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(size, nameof(size));

            X = x;
            Y = y;
            Size = size;
            _treeType = treeType;
        }

        // Render için extrinsic verileri flyweight'e geçiriyor
        public void Render()
            => _treeType.Render(X, Y, Size);

        public string GetTreeTypeName()
            => _treeType.Name;
    }
}
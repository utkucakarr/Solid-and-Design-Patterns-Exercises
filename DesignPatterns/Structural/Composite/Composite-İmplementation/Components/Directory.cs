using Composite_İmplementation.Interfaces;

namespace Composite_İmplementation.Components
{
    // Composite (Bileşik) - alt öğeler içerebilen klasör
    public sealed class Directory : IFileSystemItem
    {
        public string Name { get; }

        // Hem dosya hem klasör aynı interface - tip kontrolü yok!
        private readonly List<IFileSystemItem> _children = new();
        private IFileSystemItem? _parent;

        public Directory(string name)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name, nameof(name));
            Name = name;
        }

        public void Add(IFileSystemItem item)
        {
            ArgumentNullException.ThrowIfNull(item, nameof(item));

            item.SetParent(this);
            _children.Add(item);
        }

        public void Remove(IFileSystemItem item)
        {
            ArgumentNullException.ThrowIfNull(item, nameof(item));

            if (_children.Remove(item))
                item.SetParent(null);
        }

        public IReadOnlyCollection<IFileSystemItem> GetChildren() => _children.AsReadOnly();

        public int ChildCount => _children.Count;

        public IFileSystemItem? GetParent() => _parent;

        // Recursive boyut hesaplama - tip kontrolü yok!
        public long GetSize() => _children.Sum(child => child.GetSize());

        public void Print(string indent = "")
        {
            Console.WriteLine($"{indent} {Name} ({GetSize():N0} bytes)");

            foreach (var child in _children)
                child.Print(indent + "  ");
        }

        public void SetParent(IFileSystemItem? parent) => _parent = parent;

        public bool Contain(IFileSystemItem item)
            => _children.Contains(item);

        public override string ToString() => $"Directory[{Name}, {ChildCount} items]";
    }
}

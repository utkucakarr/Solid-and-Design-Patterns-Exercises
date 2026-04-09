using Composite_İmplementation.Interfaces;

namespace Composite_İmplementation.Components
{
    // Leaf (yaprak) - alt öğesi olmayan, boyutu belli dosya
    public sealed class File : IFileSystemItem
    {
        public string Name { get; }
        public long SizeInBytes { get; }

        private IFileSystemItem? _parent;

        public File(string name, long sizeInBytes)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name, nameof(name));
            ArgumentOutOfRangeException.ThrowIfNegative(sizeInBytes, nameof(sizeInBytes));

            Name = name;
            SizeInBytes = sizeInBytes;
        }

        public IFileSystemItem? GetParent()
            => _parent;

        // Dosyanın boyutu kendi boyutu
        public long GetSize() => SizeInBytes;

        public void Print(string indent = "")
            => Console.WriteLine($"{indent} {Name} ({SizeInBytes:N0} bytes)");

        public void SetParent(IFileSystemItem? parent)
            => _parent = parent;

        public override string ToString()
            => $"File[{Name}, {SizeInBytes} bytes]";
    }
}

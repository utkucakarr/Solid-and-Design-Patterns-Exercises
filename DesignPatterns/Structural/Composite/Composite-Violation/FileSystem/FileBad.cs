namespace Composite_Violation.FileSystem
{
    // Composite ihlali - dosya ve klasör farklı tipte tutulup
    // her yerde tip kontrolü yapılıyor.
    public class FileBad
    {
        public string Name { get; }
        public long SizeInBytes { get; }

        public FileBad(string name, long sizeInBytes)
        {
            Name = name;
            SizeInBytes = sizeInBytes;
        }
    }
}

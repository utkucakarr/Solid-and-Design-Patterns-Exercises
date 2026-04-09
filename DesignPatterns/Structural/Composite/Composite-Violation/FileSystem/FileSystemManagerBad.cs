namespace Composite_Violation.FileSystem
{

    // Client dosya mı klasör mü diye her yerde kontrol etmek zorunda
    public class FileSystemManagerBad
    {
        // Dosya ve klasörler ayrı listede - tip kontrolü şart
        private readonly List<FileBad> _files = new();
        private readonly List<DirectoryBad> _dirs = new();

        public void Add(Object item)
        {
            if (item is FileBad file)
                _files.Add(file);
            else if (item is DirectoryBad dir)
                _dirs.Add(dir);
            // Yeni tip gelirse buraya yeni else if!
        }

        // Her metotta tip kontrolü tekrarlanıyor
        public long CalculateTotalSize()
        {
            long total = 0;

            foreach (var file in _files)
                total += file.SizeInBytes;

            // Recursive hesaplama için tekrar tip kontrolü
            foreach (var dir in _dirs)
                total += CalculateDirectorySize(dir);

            return total;
        }

        private long CalculateDirectorySize(DirectoryBad dir)
        {
            long total = dir.Files.Sum(f => f.SizeInBytes);

            foreach (var subDir in dir.SubDirs)
                total += CalculateDirectorySize(subDir);

            return total;
        }

        public void Print(string indent = "")
        {
            foreach (var file in _files)
                Console.WriteLine($"{indent} {file.Name} ({file.SizeInBytes} bytes)");

            foreach (var dir in _dirs)
                PrintDirectory(dir, indent);
        }

        private void PrintDirectory(DirectoryBad dir, string indent)
        {
            Console.WriteLine($"{indent} {dir.Name}");

            foreach (var file in dir.Files)
                Console.WriteLine($"{indent} {file.Name} ({file.SizeInBytes} bytes)");

            foreach (var subDir in dir.SubDirs)
                PrintDirectory(subDir, indent + "  ");
        }
    }
}

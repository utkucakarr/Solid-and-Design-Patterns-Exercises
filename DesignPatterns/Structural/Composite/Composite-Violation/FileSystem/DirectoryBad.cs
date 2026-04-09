namespace Composite_Violation.FileSystem
{
    public class DirectoryBad
    {
        public string Name { get; }
        public List<FileBad> Files { get; } = new();
        public List<DirectoryBad> SubDirs { get; } = new();

        public DirectoryBad(string name) => Name = name;

        public void AddFile(FileBad file) => Files.Add(file);
        public void AddDirectory(DirectoryBad dir) => SubDirs.Add(dir);
    }
}

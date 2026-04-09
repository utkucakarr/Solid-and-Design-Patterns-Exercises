using Composite_İmplementation.Interfaces;

namespace Composite_İmplementation.Services
{

    // Client - IFileSystemItem üzerinde çalışıyor
    // Dosya mı klasör mü bilmek zorunda değil!
    public class FileSystemService
    {
        // Hem dosya hem klasör aynı interface - tek metot yeterli
        public long CalculateTotalSize(IFileSystemItem item)
        {
            ArgumentNullException.ThrowIfNull(item, nameof(item));
            return item.GetSize();
        }

        public void PrintStructure(IFileSystemItem item)
        {
            ArgumentNullException.ThrowIfNull(item, nameof(item));
            item.Print();
        }

        // recursive arama - tip kontrolü yok
        public IFileSystemItem? FindByName(IFileSystemItem root, string name)
        {
            ArgumentNullException.ThrowIfNull(root, nameof(root));
            ArgumentException.ThrowIfNullOrWhiteSpace(name, nameof(name));

            if (root.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                return root;

            if(root is Components.Directory directory)
            {
                foreach (var child in directory.GetChildren())
                {
                    var found = FindByName(child, name);
                    if (found is not null) return found;
                }
            }

            return null;
        }

        public List<IFileSystemItem> FindAll(
            IFileSystemItem root,
            Func<IFileSystemItem, bool> predicate)
        {
            ArgumentNullException.ThrowIfNull(root, nameof(root));
            ArgumentNullException.ThrowIfNull(predicate, nameof(predicate));

            var results = new List<IFileSystemItem>();
            CollectMatching(root, predicate, results);
            return results;
        }

        private void CollectMatching(
        IFileSystemItem item,
        Func<IFileSystemItem, bool> predicate,
        List<IFileSystemItem> results)
        {
            if (predicate(item))
                results.Add(item);

            if (item is Components.Directory directory)
            {
                foreach (var child in directory.GetChildren())
                    CollectMatching(child, predicate, results);
            }
        }
    }
}

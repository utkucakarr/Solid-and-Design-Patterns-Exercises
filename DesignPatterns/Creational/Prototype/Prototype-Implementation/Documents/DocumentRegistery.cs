namespace Prototype_Implementation.Documents
{
    // Prototype Registry — şablonları merkezi olarak yönetiyor
    public class DocumentRegistery
    {
        private readonly Dictionary<string, object> _prototypes = new();
        
        public void Register(string key, object prototype)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(key, nameof(key));
            ArgumentNullException.ThrowIfNull(prototype, nameof(prototype));

            _prototypes[key] = prototype;
            Console.WriteLine($"[Registry] '{key}' şablonu kaydedildi.");
        }

        public ReportDocument CloneReport(string key)
        {
            if (!_prototypes.TryGetValue(key, out var prototype))
                throw new KeyNotFoundException($"'{key}' şablonu bulunamadı.");

            if(prototype is not ReportDocument report)
                throw new InvalidCastException($"'{key}' bir ReportDocument değil.");

            return report.DeepClone();
        }

        public InvoiceDocument CloneInvoice(string key)
        {
            if (!_prototypes.TryGetValue(key, out var prototype))
                throw new KeyNotFoundException($"'{key}' şablonu bulunamadı.");

            if (prototype is not InvoiceDocument invoice)
                throw new InvalidCastException($"'{key}' bir InvoiceDocument değil.");

            return invoice.DeepClone();
        }

        public ContractDocument CloneContract(string key)
        {
            if (_prototypes.TryGetValue(key, out var prototype))
                throw new KeyNotFoundException($"'{key}' şablonu bulunamadı.");

            if(prototype is not ContractDocument contract)
                throw new InvalidCastException($"'{key}' bir ContractDocument değil.");

            return contract.DeepClone();
        }

        public bool Contains(string key) => _prototypes.ContainsKey(key);
        public int Count => _prototypes.Count;
    }
}

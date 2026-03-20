using LSP_Implementation.Interfaces;

namespace LSP_Implementation.Employees
{
    // Sözleşmeli: maaş + prim, fazla mesai yok
    public class Contractor : IEmployee, IBonusEligible
    {
        public string Name { get; }
        public decimal BaseSalary { get; }

        public Contractor(string name, decimal baseSalary)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name, nameof(name));
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(baseSalary, nameof(baseSalary));

            Name = name;
            BaseSalary = baseSalary;
        }

        public decimal CalculateSalary() => BaseSalary;
        public decimal CalculateBonus() => BaseSalary * 0.10m;
    }
}

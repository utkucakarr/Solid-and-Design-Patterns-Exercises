using LSP_Implementation.Interfaces;

namespace LSP_Implementation.Employees
{
    // Tam zamanlı: maaş + prim + fazla mesai — hepsini implemente eder
    public class FullTimeEmployee : IEmployee, IBonusEligible, IOvertimeEligible
    {
        public string Name { get; }
        public decimal BaseSalary { get; }

        public FullTimeEmployee(string name, decimal baseSalary)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name, nameof(name));
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(baseSalary, nameof(baseSalary));

            Name = name;
            BaseSalary = baseSalary;
        }

        public decimal CalculateSalary() => BaseSalary;
        public decimal CalculateBonus() => BaseSalary * 0.20m;
        public decimal CalculateOvertime() => BaseSalary * 0.10m;
    }
}

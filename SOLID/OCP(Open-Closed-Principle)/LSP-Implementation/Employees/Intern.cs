using LSP_Implementation.Interfaces;

namespace LSP_Implementation.Employees
{
    public class Intern : IEmployee
    {
        public string Name { get; }
        public decimal BaseSalary { get; }

        public Intern(string name, decimal baseSalary)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name, nameof(name));
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(baseSalary, nameof(baseSalary));

            Name = name;
            BaseSalary = baseSalary;
        }

        public decimal CalculateSalary() => BaseSalary;
    }
}

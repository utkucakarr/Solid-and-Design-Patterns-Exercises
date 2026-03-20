namespace LSP_Violation
{
    public class FullTimeEmployee : Employee
    {
        public FullTimeEmployee(string name, decimal baseSalary) : base(name, baseSalary)
        {
        }

        public override decimal CalculateSalary() => BaseSalary;
        public override decimal CalculateBonus() => BaseSalary * 0.20m;
        public override decimal CalculateOvertime() => BaseSalary * 0.10m;
    }
}

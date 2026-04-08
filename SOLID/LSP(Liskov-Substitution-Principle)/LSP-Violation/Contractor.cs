namespace LSP_Violation
{
    public class Contractor : Employee
    {
        public Contractor(string name, decimal baseSalary)
    : base(name, baseSalary) { }

        public override decimal CalculateSalary() => BaseSalary;
        public override decimal CalculateBonus() => BaseSalary * 0.10m;

        // LSP İHLALİ — Sözleşmeli fazla mesai alamaz!
        public override decimal CalculateOvertime()
            => throw new NotSupportedException("Sözleşmeliler fazla mesai alamaz!");
    }
}

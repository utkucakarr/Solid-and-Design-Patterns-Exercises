namespace LSP_Violation
{
    public class Intern : Employee
    {
        public Intern(string name, decimal baseSalary)
    : base(name, baseSalary) { }

        public override decimal CalculateSalary() => BaseSalary;

        // 💥 LSP İHLALİ — Stajyer prim alamaz!
        public override decimal CalculateBonus()
            => throw new NotSupportedException("Stajyerler prim alamaz!");

        // 💥 LSP İHLALİ — Stajyer fazla mesai alamaz!
        public override decimal CalculateOvertime()
            => throw new NotSupportedException("Stajyerler fazla mesai alamaz!");
    }
}

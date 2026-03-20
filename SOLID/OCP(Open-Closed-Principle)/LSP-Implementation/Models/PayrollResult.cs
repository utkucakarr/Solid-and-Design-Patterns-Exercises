namespace LSP_Implementation.Models
{
    public class PayrollResult
    {
        public string EmployeeName { get; }
        public decimal Salary { get; }
        public decimal Bonus { get; }
        public decimal Overtime { get; }
        public decimal TotalPayment => Salary + Bonus + Overtime;

        private PayrollResult(string employeeName, decimal salary, decimal bonus, decimal overtime)
        {
            EmployeeName = employeeName;
            Salary = salary;
            Bonus = bonus;
            Overtime = overtime;
        }

        public static PayrollResult Create(string employeeName, decimal salary, decimal bonus = 0, decimal overtime = 0)
            => new(employeeName, salary, bonus, overtime);
    }
}

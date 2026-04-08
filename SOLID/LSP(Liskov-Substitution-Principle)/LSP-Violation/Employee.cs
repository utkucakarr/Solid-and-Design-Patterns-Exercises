namespace LSP_Violation
{

    // Abstract class herkese aynı sözleşmeyi dayatıyor
    public abstract class Employee
    {
        public string Name { get; }
        public decimal BaseSalary { get; }

        protected Employee(string name, decimal baseSalary)
        {
            Name = name;
            BaseSalary = baseSalary;
        }

        public abstract decimal CalculateSalary();
        public abstract decimal CalculateBonus();    // Her çalışan prim alabilir mi? HAYIR!
        public abstract decimal CalculateOvertime(); // Her çalışan fazla mesai alabilir mi? HAYIR!
    }
}

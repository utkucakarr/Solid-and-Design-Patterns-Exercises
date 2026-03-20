namespace LSP_Implementation.Interfaces
{
    // Tüm çalışanların ortak sözleşmesi — sadece maaş
    public interface IEmployee
    {
        string Name { get; }
        decimal BaseSalary { get; }
        decimal CalculateSalary();
    }
}

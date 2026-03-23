namespace LSP_Implementation.Interfaces
{
    // Sadece fazla mesai alabilecek çalışanlar bu sözleşmeyi uygular
    public interface IOvertimeEligible
    {
        decimal CalculateOvertime();
    }
}

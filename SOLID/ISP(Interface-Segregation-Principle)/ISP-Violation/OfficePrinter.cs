namespace ISP_Violation
{
    public class OfficePrinter : IPrinter
    {
        public void Fax(string document)
            => throw new NotSupportedException("OfficePrinter fax yapamaz!");

        public void Print(string document)
            => Console.WriteLine($"[PRINT] '{document}' yazdırıldı.");

        public void Scan(string document)
            => Console.WriteLine($"[SCAN] '{document}' tarandı.");
    }
}

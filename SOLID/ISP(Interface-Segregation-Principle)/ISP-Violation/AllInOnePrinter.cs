namespace ISP_Violation
{
    public class AllInOnePrinter : IPrinter
    {
        public void Print(string document)
            => Console.WriteLine($"[PRINT] '{document}' yazdırıldı.");

        public void Scan(string document)
            => Console.WriteLine($"[SCAN] '{document}' tarandı.");

        public void Fax(string document)
            => Console.WriteLine($"[FAX] '{document}' fakslandı.");
    }
}

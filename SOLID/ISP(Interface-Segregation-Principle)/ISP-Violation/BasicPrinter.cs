namespace ISP_Violation
{
    public class BasicPrinter : IPrinter
    {
        public void Fax(string document)
            => throw new NotSupportedException("BasicPrinter fax yapamaz!");

        // ISP İHLALİ — Faks da desteklenmiyor
        public void Print(string document)
            => throw new NotSupportedException("BasicPrinter tarama yapamaz!");
        
        // ISP İHLALİ — Sadece yazdırabilen yazıcı taramak zorunda bırakılıyor
        public void Scan(string document)
            => Console.WriteLine($"[PRINT] '{document}' yazdırıldı.");
    }
}

namespace Adapter_Violation.Payment
{
    public class IyzicoServiceBad
    {
        public int OdemeYap(decimal tutar, string paraBirimi)
        {
            Console.WriteLine($"[Iyzico] {tutar} {paraBirimi} ödendi.");
            return 200; // HTTP 200 OK.
        }
    }
}

namespace Adapter_Implementation.ThirdParty
{
    // Iyzico'nun kendi API'si — Türkçe metod isimleri, int HTTP kodu döner
    public class IyzicoService
    {
        public int OdemeYap(decimal tutar, string paraBirimi)
        {
            Console.WriteLine($"[Iyzico SDK] OdemeYap({tutar}, {paraBirimi})");
            return 200; // HTTP 200 = başarılı
        }

        public int IadeYap(string referansKodu, decimal tutar)
        {
            Console.WriteLine($"[Iyzico SDK] IadeYap({referansKodu}, {tutar})");
            return 200;
        }

        public string ReferansKoduOlustur()
            => $"IYZ_{Guid.NewGuid():N}";
    }
}

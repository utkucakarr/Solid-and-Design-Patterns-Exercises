using Command_Violation.Devices;

namespace Command_Violation
{
    public class SmartHomeControllerBad
    {

        // Tüm cihazlar doğrudan controller'a bağımlı — command soyutlaması yok
        private readonly LightBad _livingRoomLight;
        private readonly LightBad _bedroomLight;
        private readonly ThermostatBad _thermostat;
        private readonly SecurityCameraBad _camera;

        // 🚨 Geçmiş tutulmuyor — undo/redo tamamen imkânsız
        private readonly List<string> _log = new();

        public SmartHomeControllerBad()
        {
            _livingRoomLight = new LightBad("Oturma Odası");
            _bedroomLight = new LightBad("Yatak Odası");
            _thermostat = new ThermostatBad();
            _camera = new SecurityCameraBad();
        }

        // Her cihaz işlemi için controller'a yeni metot ekleniyor (OCP ihlali)
        public string TurnOnLivingRoomLight()
        {
            _livingRoomLight.TurnOn();
            var msg = $"[BAD] Oturma odası ışığı açıldı. Durum: {_livingRoomLight.IsOn}";
            _log.Add(msg);
            return msg;
        }

        public string TurnOffLivingRoomLight()
        {
            // Önceki durum hiç kaydedilmedi — geri alma imkânsız
            _livingRoomLight.TurnOff();
            var msg = $"[BAD] Oturma odası ışığı kapatıldı. Durum: {_livingRoomLight.IsOn}";
            _log.Add(msg);
            return msg;
        }

        public string SetTemperature(int temperature)
        {
            try
            {
                // Önceki sıcaklık saklanmadığı için undo yapılamaz
                _thermostat.SetTemperature(temperature);
                var msg = $"[BAD] Sıcaklık {temperature}°C olarak ayarlandı.";
                _log.Add(msg);
                return msg;
            }
            catch (ArgumentOutOfRangeException ex)
            {
                // 2. Eğer Termostat "bu değer bana uygun değil" derse, hata burada yakalanır.
                var errorMsg = $"[HATA] Sıcaklık ayarlanamadı: {ex.Message}";
                _log.Add(errorMsg);
                return errorMsg;
            }
        }

        public string ArmCamera()
        {
            _camera.Arm();
            var msg = "[BAD] Güvenlik kamerası aktif edildi.";
            _log.Add(msg);
            return msg;
        }

        // Makro işlem hardcoded — yeni cihaz eklenince bu metot değişmek zorunda (OCP ihlali)
        public List<string> GoodNightMode()
        {
            var results = new List<string>();

            _livingRoomLight.TurnOff();
            results.Add("[BAD] Oturma odası ışığı söndürüldü.");

            _bedroomLight.TurnOff();
            results.Add("[BAD] Yatak odası ışığı söndürüldü.");

            _camera.Arm();
            results.Add("[BAD] Güvenlik kamerası aktif edildi.");

            // Bu 3 işlemin tamamını tek seferde geri almak imkânsız
            return results;
        }

        // Undo / Redo metodu hiç yok — işlemler kalıcı
        public List<string> GetLog() => _log.AsReadOnly().ToList();
    }
}

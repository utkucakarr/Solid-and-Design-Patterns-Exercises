namespace Command_Implementation.Devices
{
    public class Thermostat
    {
        private const int MinTemperature = -20;
        private const int MaxTemperature = 50;

        public int Temperature { get; private set; } = 20;
        public string Name => "Termostat";

        // Önceki değeri döndürüyor — SetTemperatureCommand undo için bunu kullanacak
        public int SetTemperature(int temperature)
        {
            if(temperature < MinTemperature || temperature > MaxTemperature)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(temperature), 
                    $"Sıcaklık değeri {MinTemperature} ile {MaxTemperature} arasında olmalıdır. Girilen değer: {temperature}");
            }

            var previous = Temperature;
            Temperature = temperature;
            return previous;
        }
    }
}

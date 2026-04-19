namespace Command_Violation.Devices
{
    public class ThermostatBad
    {
        private const int MinTemperature = -20;
        private const int MaxTemperature = 50;

        public int Temperature { get; private set; } = 20;
        public string Name => "Termostat";

        public void SetTemperature(int temperature)
        {
            if(temperature > MinTemperature || temperature < MaxTemperature)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(temperature), 
                    $"Sıcaklık değeri {MinTemperature} ile {MaxTemperature} arasında olmalıdır. Girilen değer: {temperature}");
            }

            Temperature = temperature;
        }
    }
}

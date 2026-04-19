using Command_Implementation.Devices;
using Command_Implementation.Interfaces;
using Command_Implementation.Models;

namespace Command_Implementation.Commands
{
    public class SetTemperatureCommand : ICommand
    {
        private readonly Thermostat _thermostat;
        private readonly int _newTemperature;
        private int _previousTemperature;

        public string Description => $"Sıcaklığı {_newTemperature}°C olarak ayarla";

        public SetTemperatureCommand(Thermostat thermostat, int newTemperature)
        {
            ArgumentNullException.ThrowIfNull(thermostat, nameof(thermostat));
            _thermostat = thermostat;
            _newTemperature = newTemperature;
        }

        public CommandResult Execute()
        {
            // Önceki değer kaydediliyor — undo için kritik
            try
            {
                _previousTemperature = _thermostat.SetTemperature(_newTemperature);
                return CommandResult.Success(
                    $"Sıcaklık {_newTemperature}°C olarak ayarlandı (önceki: {_previousTemperature}°C).",
                    _thermostat.Name,
                    "SetTemperature");
            }
            catch (ArgumentOutOfRangeException ex)
            {
                return CommandResult.Fail($"Sıcaklık ayarlanamadı: {ex.Message}");
            }

        }

        public CommandResult Undo()
        {
            // Önceki sıcaklığa tam olarak geri dönülüyor
            _thermostat.SetTemperature(_previousTemperature);
            return CommandResult.Success(
                $"Sıcaklık {_previousTemperature}°C'ye geri alındı.",
                _thermostat.Name,
                "UndoSetTemperature");
        }
    }
}

using Command_Implementation.Devices;
using Command_Implementation.Interfaces;
using Command_Implementation.Models;

namespace Command_Implementation.Commands
{
    public class TurnOffLightCommand : ICommand
    {
        private readonly Light _light;
        public string Description => $"{_light.Room} ışığını kapat";

        public TurnOffLightCommand(Light light)
        {
            ArgumentNullException.ThrowIfNull(light, nameof(light));
            _light = light;
        }

        public CommandResult Execute()
        {
            _light.TurnOff();
            return CommandResult.Success($"{_light.Room} ışığı kapatıldı.", _light.Room, "TurnOff");
        }

        public CommandResult Undo()
        {
            _light.TurnOn();
            return CommandResult.Success($"{_light.Room} ışığı (undo) açıldı.", _light.Room, "TurnOn");
        }
    }
}

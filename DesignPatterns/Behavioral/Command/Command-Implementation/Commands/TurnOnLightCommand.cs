using Command_Implementation.Devices;
using Command_Implementation.Interfaces;
using Command_Implementation.Models;

namespace Command_Implementation.Commands
{
    public class TurnOnLightCommand : ICommand
    {
        private readonly Light _light;

        public string Description => $"{_light.Room} ışığını aç";

        public TurnOnLightCommand(Light light)
        {
            ArgumentNullException.ThrowIfNull(light, nameof(light));
            _light = light;
        }

        public CommandResult Execute()
        {
            _light.TurnOn();
            return CommandResult.Success($"{_light.Room} ışığı açıldı.", _light.Room, "TurnOn");
        }

        public CommandResult Undo()
        {
            _light.TurnOff();
            return CommandResult.Success($"{_light.Room} ışığı (undo) kapatıldı.", _light.Room, "TurnOff");
        }
    }
}

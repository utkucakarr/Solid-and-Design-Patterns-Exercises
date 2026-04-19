using Command_Implementation.Devices;
using Command_Implementation.Interfaces;
using Command_Implementation.Models;

namespace Command_Implementation.Commands
{
    public class ArmCameraCommand : ICommand
    {
        private readonly SecurityCamera _camera;

        public string Description => "Güvenlik kamerasını aktif et";

        public ArmCameraCommand(SecurityCamera camera)
        {
            ArgumentNullException.ThrowIfNull(camera, nameof(camera));
            _camera = camera;
        }

        public CommandResult Execute()
        {
            _camera.Arm();
            return CommandResult.Success("Güvenlik kamerası aktif edildi.", _camera.Name, "Arm");
        }

        public CommandResult Undo()
        {
            _camera.Disarm();
            return CommandResult.Success("Güvenlik kamerası (undo) devre dışı bırakıldı.", _camera.Name, "Disarm");
        }
    }
}

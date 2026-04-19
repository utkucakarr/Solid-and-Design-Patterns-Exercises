namespace Command_Implementation.Devices
{
    public class SecurityCamera
    {
        public bool IsArmed { get; private set; }
        public string Name => "Güvenlik Kamerası";

        // Simetrik Arm / Disarm — ArmCameraCommand undo için Disarm'ı çağırır
        public void Arm() => IsArmed = true;
        public void Disarm() => IsArmed = false;
    }
}

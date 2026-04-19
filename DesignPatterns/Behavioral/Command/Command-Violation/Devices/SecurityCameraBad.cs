namespace Command_Violation.Devices
{
    public class SecurityCameraBad
    {
        public bool IsArmed { get; private set; }
        public string Name => "Güvenlik Kamerası";

        public void Arm() => IsArmed = true;
        public void Disarm() => IsArmed = false;
    }
}

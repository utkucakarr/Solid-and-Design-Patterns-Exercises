namespace Command_Violation.Devices
{
    public class LightBad
    {
        public string Room { get; }
        public bool IsOn { get; private set; }

        public LightBad(string room)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(room, nameof(room));
            Room = room;
        }

        public void TurnOn() => IsOn = true;
        public void TurnOff() => IsOn = false;
    }
}

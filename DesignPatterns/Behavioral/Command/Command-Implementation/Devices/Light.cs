namespace Command_Implementation.Devices
{
    public class Light
    {
        public string Room { get; }
        public bool IsOn { get; private set; }

        public Light(string room)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(room, nameof(room));
            Room = room;
        }

        // Cihaz yalnızca kendi durumunu yönetiyor — komut mantığı burada yok
        public void TurnOn() => IsOn = true;
        public void TurnOff() => IsOn = false;
    }
}

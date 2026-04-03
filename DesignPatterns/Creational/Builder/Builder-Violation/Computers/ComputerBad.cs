namespace Builder_Violation.Computers
{
    // Telescoping Constructor — parametre sayısı arttıkça
    //    hangi parametrenin ne olduğu anlaşılamıyor
    public class ComputerBad
    {
        public string CPU { get; }
        public int RamGB { get; }
        public int StorageGB { get; }
        public string GPU { get; }
        public string OS { get; }
        public bool HasWifi { get; }
        public bool HasBluetooth { get; }
        public string CoolingSystem { get; }
        public string PowerSupply { get; }
        public string Motherboard { get; }

        // Telescoping Constructor — 10 parametre!
        //    Hangi bool neyi temsil ediyor? Sıra karışırsa hata!
        public ComputerBad(
            string cpu,
            int ramGB,
            int storageGB,
            string gpu,
            string os,
            bool hasWifi,
            bool hasBluetooth,
            string coolingSystem,
            string powerSupply,
            string motherborad)
        {
            CPU = cpu;
            RamGB = ramGB;
            StorageGB = storageGB;
            GPU = gpu;
            OS = os;
            HasWifi = hasWifi;
            HasBluetooth = hasBluetooth;
            CoolingSystem = coolingSystem;
            PowerSupply = powerSupply;
            Motherboard = motherborad;
        }
    }
}
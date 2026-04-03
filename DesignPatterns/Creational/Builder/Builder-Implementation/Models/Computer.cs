using System.Runtime.Intrinsics.Arm;

namespace Builder_Implementation.Models
{
    public class Computer
    {
        public string CPU { get; init; } = string.Empty;
        public int RamGB { get; init; }
        public int StorageGB { get; init; }
        public string GPU { get; init; } = string.Empty;
        public string OS { get; init; } = string.Empty;
        public bool HasWifi { get; init; }
        public bool HasBluetooth { get; init; }
        public string CoolingSystem { get; init; } = string.Empty;
        public string PowerSupply { get; init; } = string.Empty;
        public string Motherboard { get; init; } = string.Empty;

        // Private constructor — sadece Builder oluşturabilir
        internal Computer() { }

        public override string ToString()
        {
            return $"Motherboard: {Motherboard} , CPU: {CPU} , RAM: {RamGB}GB , Storage: {StorageGB}GB , " +
                   $"GraphicsCard: {GPU} , OS: {OS} , Cooling: {CoolingSystem} , PowerSupply: {PowerSupply} , " +
                   $"WiFi: {(HasWifi ? "Var" : "Yok")} , Bluetooth: {(HasBluetooth ? "Var" : "Yok")}";
        }
    }
}
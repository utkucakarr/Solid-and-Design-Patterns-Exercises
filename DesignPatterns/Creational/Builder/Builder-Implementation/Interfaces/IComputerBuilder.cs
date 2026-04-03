using Builder_Implementation.Models;

namespace Builder_Implementation.Interfaces
{
    // Builder sözleşmesi — adım adım inşa
    public interface IComputerBuilder
    {
        IComputerBuilder SetCPU(string cpu);
        IComputerBuilder SetRAM(int ramGB);
        IComputerBuilder SetStorage(int storageGB);
        IComputerBuilder SetGPU(string gpu);
        IComputerBuilder SetOS(string os);
        IComputerBuilder SetWifi(bool hasWifi);
        IComputerBuilder SetBluetooth(bool hasBluetooth);
        IComputerBuilder SetCoolingSystem(string coolingSystem);
        IComputerBuilder SetPowerSupply(string powerSupply);
        IComputerBuilder SetMotherboard(string motherboard);
        Computer Build();
        void Reset();
    }
}

using Builder_Implementation.Interfaces;
using Builder_Implementation.Models;

namespace Builder_Implementation.Builders
{
    public class GamingComputerBuilder : IComputerBuilder
    {
        private string _cpu = string.Empty;
        private int _ramGB;
        private int _storageGB;
        private string _gpu = string.Empty;
        private string _os = string.Empty;
        private bool _hasWifi;
        private bool _hasBluetooth;
        private string _coolingSystem = string.Empty;
        private string _powerSupply = string.Empty;
        private string _motherboard = string.Empty;

        // Tüm parçaları bir araya getirip Computer oluşturuyor
        public Computer Build()
        {
            var computer = new Computer()
            {
                CPU = _cpu,
                RamGB = _ramGB,
                StorageGB = _storageGB,
                GPU = _gpu,
                OS = _os,
                HasWifi = _hasWifi,
                HasBluetooth = _hasBluetooth,
                CoolingSystem = _coolingSystem,
                PowerSupply = _powerSupply,
                Motherboard = _motherboard
            };

            Reset();
            return computer;
        }

        public void Reset()
        {
            _cpu = string.Empty;
            _ramGB = 0;
            _storageGB = 0;
            _gpu = string.Empty;
            _os = string.Empty;
            _hasWifi = false;
            _hasBluetooth = false;
            _coolingSystem = string.Empty;
            _powerSupply = string.Empty;
            _motherboard = string.Empty;
        }

        public IComputerBuilder SetBluetooth(bool hasBluetooth)
        {
            _hasBluetooth = hasBluetooth;
            return this; // Method chaining
        }

        public IComputerBuilder SetCoolingSystem(string coolingSystem)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(coolingSystem, nameof(coolingSystem));
            _coolingSystem = coolingSystem;
            return this;
        }

        public IComputerBuilder SetCPU(string cpu)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(cpu, nameof(cpu));
            _cpu = cpu;
            return this;
        }

        public IComputerBuilder SetGPU(string gpu)
        {

            ArgumentException.ThrowIfNullOrWhiteSpace(gpu, nameof(gpu));
            _gpu = gpu;
            return this;
        }

        public IComputerBuilder SetMotherboard(string motherboard)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(motherboard, nameof(motherboard));
            _motherboard = motherboard;
            return this;
        }

        public IComputerBuilder SetOS(string os)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(os, nameof(os));
            _os = os;
            return this;
        }

        public IComputerBuilder SetPowerSupply(string powerSupply)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(powerSupply, nameof(powerSupply));
            _powerSupply = powerSupply;
            return this;
        }

        public IComputerBuilder SetRAM(int ramGB)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(ramGB, nameof(ramGB));
            _ramGB = ramGB;
            return this;
        }

        public IComputerBuilder SetStorage(int storageGB)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(storageGB, nameof(storageGB));
            _storageGB = storageGB;
            return this;
        }

        public IComputerBuilder SetWifi(bool hasWifi)
        {
            _hasWifi = hasWifi;
            return this;
        }
    }
}
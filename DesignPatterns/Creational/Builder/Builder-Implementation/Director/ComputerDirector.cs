using Builder_Implementation.Interfaces;
using Builder_Implementation.Models;

namespace Builder_Implementation.Director
{
    // Director — inşa adımlarını belirli bir sıraya koyar
    // Client builder'ı nasıl kullanacağını bilmek zorunda değil
    public class ComputerDirector
    {
        private IComputerBuilder _computerBuilder;

        public ComputerDirector(IComputerBuilder computerBuilder)
        {
            _computerBuilder = computerBuilder
                ?? throw new ArgumentNullException(nameof(computerBuilder));
        }

        public void ChangeBuilder(IComputerBuilder computerBuilder)
        {
            _computerBuilder = computerBuilder
                ?? throw new ArgumentNullException(nameof(computerBuilder));
        }

        // Standart gaming bilgisayar konfigürasyonu
        public Computer BuildGamingComputer()
        {
            return _computerBuilder
                .SetCPU("Intel Core i9-13900K")
                .SetRAM(32)
                .SetStorage(2000)
                .SetGPU("NVIDIA RTX 4090")
                .SetOS("Windows 11 Pro")
                .SetWifi(true)
                .SetBluetooth(true)
                .SetCoolingSystem("Liquid Cooling 360mm")
                .SetPowerSupply("1000W 80+ Gold")
                .SetMotherboard("ASUS ROG Maximus Z790")
                .Build();
        }

        // Standart ofis bilgisayar konfigürasyonu
        public Computer BuildOfficeComputer()
        {
            return _computerBuilder
                .SetCPU("Intel Core i5-13400")
                .SetRAM(16)
                .SetStorage(512)
                .SetOS("Windows 11 Home")
                .SetWifi(true)
                .SetBluetooth(true)
                .SetCoolingSystem("Standart Fan")
                .SetPowerSupply("500W 80+ Bronze")
                .SetMotherboard("MSI PRO B660M")
                .Build();
        }

        // Standart sunucu konfigürasyonu
        public Computer BuildServerComputer()
        {
            return _computerBuilder
                .SetCPU("AMD EPYC 7763")
                .SetRAM(256)
                .SetStorage(10000)
                .SetOS("Ubuntu Server 22.04")
                .SetWifi(false)
                .SetBluetooth(false)
                .SetCoolingSystem("Server Soğutma Sistemi")
                .SetPowerSupply("2000W Redundant")
                .SetMotherboard("Supermicro H12SSL")
                .Build();
        }
    }
}
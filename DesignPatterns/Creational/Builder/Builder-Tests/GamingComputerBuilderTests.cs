using Builder_Implementation.Builders;
using FluentAssertions;

namespace Builder_Tests
{
    public class GamingComputerBuilderTests
    {
        private readonly GamingComputerBuilder _sut = new();

        // --- Build Testleri ---
        [Fact]
        public void Build_ShouldReturnComputer_WithCorrectCPU()
        {
            var computer = _sut
            .SetCPU("Intel i9")
            .SetRAM(32)
            .SetStorage(1000)
            .SetGPU("RTX 4090")
            .SetOS("Windows 11")
            .SetWifi(true)
            .SetBluetooth(true)
            .SetCoolingSystem("Liquid")
            .SetPowerSupply("1000W")
            .SetMotherboard("ASUS ROG")
            .Build();

            computer.CPU.Should().Be("Intel i9");
        }

        [Fact]
        public void Build_ShouldReturnComputer_WithCorrectRAM()
        {
            var computer = _sut
                .SetCPU("Intel i9")
                .SetRAM(32)
                .SetStorage(1000)
                .SetGPU("RTX 4090")
                .SetOS("Windows 11")
                .SetWifi(true)
                .SetBluetooth(true)
                .SetCoolingSystem("Liquid")
                .SetPowerSupply("1000W")
                .SetMotherboard("ASUS ROG")
                .Build();

            computer.RamGB.Should().Be(32);
        }

        [Fact]
        public void Build_ShouldReturnComputer_WithWifiEnabled()
        {
            var computer = _sut
                .SetCPU("Intel i9")
                .SetRAM(32)
                .SetStorage(1000)
                .SetGPU("RTX 4090")
                .SetOS("Windows 11")
                .SetWifi(true)
                .SetBluetooth(true)
                .SetCoolingSystem("Liquid")
                .SetPowerSupply("1000W")
                .SetMotherboard("ASUS ROG")
                .Build();

            computer.HasWifi.Should().BeTrue();
        }

        // --- Method Chaining Testleri ---

        [Fact]
        public void SetCPU_ShouldReturnBuilderInstance_ForChaining()
        {
            var result = _sut.SetCPU("Intel i9");

            result.Should().BeSameAs(_sut);
        }

        [Fact]
        public void SetRAM_ShouldReturnBuilderInstance_ForChaining()
        {
            var result = _sut.SetRAM(32);

            result.Should().BeSameAs(_sut);
        }

        // --- Reset Testleri ---

        [Fact]
        public void Build_ShouldResetBuilder_AfterBuilding()
        {
            // Ýlk build
            _sut.SetCPU("Intel i9").SetRAM(32).SetStorage(1000)
                .SetGPU("RTX 4090").SetOS("Win").SetWifi(true)
                .SetBluetooth(true).SetCoolingSystem("Liquid")
                .SetPowerSupply("1000W").SetMotherboard("ASUS");
            _sut.Build();

            // Ýkinci build — önceki deðerler temizlendi
            _sut.SetCPU("AMD Ryzen").SetRAM(16).SetStorage(500)
                .SetGPU("RX 7900").SetOS("Linux").SetWifi(false)
                .SetBluetooth(false).SetCoolingSystem("Fan")
                .SetPowerSupply("600W").SetMotherboard("MSI");
            var computer = _sut.Build();

            computer.CPU.Should().Be("AMD Ryzen");
            computer.RamGB.Should().Be(16);
        }

        // --- Guard Clause Testleri ---

        [Fact]
        public void SetCPU_WithEmptyValue_ShouldThrowArgumentException()
        {
            var act = () => _sut.SetCPU(" ");

            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void SetRAM_WithNegativeValue_ShouldThrowArgumentOutOfRangeException()
        {
            var act = () => _sut.SetRAM(-1);

            act.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void SetStorage_WithZeroValue_ShouldThrowArgumentOutOfRangeException()
        {
            var act = () => _sut.SetStorage(0);

            act.Should().Throw<ArgumentOutOfRangeException>();
        }
    }
}
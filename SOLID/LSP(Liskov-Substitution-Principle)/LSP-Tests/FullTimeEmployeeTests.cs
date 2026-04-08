using FluentAssertions;
using LSP_Implementation.Interfaces;
using LSP_Implementation.Employees;

namespace LSP_Tests
{
    public class FullTimeEmployeeTests
    {
        private readonly FullTimeEmployee _fullTimeEmployee = new("Ahmet", 30000);

        // --- IEmployee ---

        [Fact]
        public void CalculateSalary_ShouldReturnBaseSalary()
        {
            _fullTimeEmployee.CalculateSalary().Should().Be(30000);
        }

        // --- IBonusEligible ---

        [Fact]
        public void CalculateBonus_ShouldReturnTwentyPercent()
        {
            _fullTimeEmployee.CalculateBonus().Should().Be(6000); // 30000 * 0.20
        }

        // --- IOvertimeEligible ---

        [Fact]
        public void CalculateOvertime_ShouldReturnTenPercent()
        {
            _fullTimeEmployee.CalculateOvertime().Should().Be(3000); // 30000 * 0.10
        }

        // --- LSP Garantisi ---

        [Fact]
        public void FullTimeEmployee_ShouldBeAssignableTo_IEmployee()
        {
            _fullTimeEmployee.Should().BeAssignableTo<IEmployee>();
        }

        [Fact]
        public void FullTimeEmployee_ShouldBeAssignableTo_IBonusEligible()
        {
            _fullTimeEmployee.Should().BeAssignableTo<IBonusEligible>();
        }

        [Fact]
        public void FullTimeEmployee_ShouldBeAssignableTo_IOvertimeEligible()
        {
            _fullTimeEmployee.Should().BeAssignableTo<IOvertimeEligible>();
        }

        [Fact]
        public void FullTimeEmployee_AsIEmployee_ShouldNotThrow()
        {
            IEmployee employee = new FullTimeEmployee("Test", 10000);

            var act = () => employee.CalculateSalary();

            act.Should().NotThrow();
        }


        [Fact]
        public void Constructor_WithEmptyName_ShouldThrowArgumentException()
        {
            var act = () => new FullTimeEmployee(" ", 30000);

            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Constructor_WithNegativeSalary_ShouldThrowArgumentOutOfRangeException()
        {
            var act = () => new FullTimeEmployee("Ahmet", -1);

            act.Should().Throw<ArgumentOutOfRangeException>();
        }
    }
}